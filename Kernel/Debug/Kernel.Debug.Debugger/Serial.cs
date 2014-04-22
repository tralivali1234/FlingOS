﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Pipes;

namespace Kernel.Debug.Debugger
{
    /// <summary>
    /// Handler for methods called when the serial pipe is connected.
    /// </summary>
    public delegate void OnConnectedHandler();

    /// <summary>
    /// A serial pipe wrapper.
    /// </summary>
    /// <remarks>
    /// Implements IDisposable to cleanly close the pipe connection.
    /// </remarks>
    public sealed class Serial : IDisposable
    {
        /// <summary>
        /// The underlying pipe.
        /// </summary>
        private NamedPipeServerStream ThePipe;
        
        /// <summary>
        /// Whether the serial pipe is connected.
        /// </summary>
        public bool Connected
        {
            get
            {
                return ThePipe != null && ThePipe.IsConnected;
            }
        }

        /// <summary>
        /// Fired when the serial pipe gains a connection.
        /// </summary>
        public event OnConnectedHandler OnConnected;

        Queue<byte> BytesRead = new Queue<byte>();
        byte[] readBuffer = new byte[512];

        /// <summary>
        /// Disposes of the serial class. Calls <see cref="Disconnect"/>.
        /// </summary>
        public void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initialises the named pipe server and waits for a connection
        /// </summary>
        /// <param name="pipe">The name of the pipe to create</param>
        /// <returns>True if a connection is received. Otherwise false.</returns>
        public bool Init(string pipe)
        {
            bool OK = Disconnect();
            if (OK)
            {
                try
                {
                    ThePipe = new NamedPipeServerStream(pipe, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                    ThePipe.BeginWaitForConnection(new AsyncCallback(delegate(IAsyncResult result)
                    {
                        try
                        {
                            ThePipe.EndWaitForConnection(result);
                            BeginRead();

                            if (OnConnected != null)
                            {
                                OnConnected();
                            }
                        }
                        catch
                        {
                            //Ignore as probably error while terminating
                        }
                    }), null);
                }
                catch
                {
                    OK = false;
                    ThePipe = null;
                }
            }
            return OK;
        }
        /// <summary>
        /// Cleanly disconnects the pipe and terminates reading.
        /// </summary>
        /// <returns>True if disconnected successfully.</returns>
        public bool Disconnect()
        {
            bool OK = true;

            if (ThePipe != null)
            {
                ThePipe.Close();
                ThePipe.Dispose();
                ThePipe = null;
            }

            return OK;
        }

        private void BeginRead()
        {
            if (Connected)
            {
                try
                {
                    ThePipe.BeginRead(readBuffer, 0, readBuffer.Length, new AsyncCallback(EndRead), null);
                }
                catch(NullReferenceException)
                {
                    //Ignore - usually occurs when closing...
                }
            }
        }
        private void EndRead(IAsyncResult result)
        {
            try
            {
                int numread = ThePipe.EndRead(result);

                if (numread > 0)
                {
                    for (int i = 0; i < numread; i++)
                    {
                        BytesRead.Enqueue(readBuffer[i]);
                    }
                }
            }
            catch
            {
            }

            if (Connected)
            {
                BeginRead();
            }
        }

        /// <summary>
        /// Reads the specified number of bytes from the pipe.
        /// </summary>
        /// <param name="numToRead">The number of bytes to read.</param>
        /// <returns>The bytes read.</returns>
        public byte[] ReadBytes(int numToRead)
        {
            do
            {
                Thread.Sleep(10);
            }
            while (BytesRead.Count < numToRead && Connected);
            
            byte[] readBuffer = new byte[numToRead];
            for (int i = 0; i < numToRead; i++)
            {
                readBuffer[i] = BytesRead.Dequeue();
            }
            return readBuffer;
        }

        /// <summary>
        /// Writes a byte to the serial pipe.
        /// </summary>
        /// <param name="aByte">The byte to write.</param>
        public void Write(byte aByte)
        {
            if(Connected)
            {
                ThePipe.Write(new byte[] { aByte }, 0, 1);
            }
        }
        /// <summary>
        /// Writes a UInt32 to the serial pipe.
        /// </summary>
        /// <param name="anInt">The UInt32 to write.</param>
        public void Write(UInt32 anInt)
        {
            if (Connected)
            {
                ThePipe.BeginWrite(BitConverter.GetBytes(anInt), 0, 4, new AsyncCallback(delegate(IAsyncResult result)
                {
                    ThePipe.EndWrite(result);
                }), null);
            }
        }
        /// <summary>
        /// Writes a UInt64 to the serial pipe.
        /// </summary>
        /// <param name="anInt">The UInt64 to write.</param>
        public void Write(UInt64 anInt)
        {
            if (Connected)
            {
                ThePipe.BeginWrite(BitConverter.GetBytes(anInt), 0, 4, new AsyncCallback(delegate(IAsyncResult result)
                {
                    ThePipe.EndWrite(result);
                }), null);
            }
        }
    }
}
