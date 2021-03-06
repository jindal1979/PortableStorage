﻿using System;
using System.IO;

namespace PortableStorage
{
    public class SyncStream : Stream
    {
        private readonly Stream _stream;
        private readonly object _lockObject;
        private long _position;

        public SyncStream(Stream stream, bool keepCurrentPosition = false)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _lockObject = stream;
            _position = keepCurrentPosition ? _stream.Position : 0;
        }

        public override bool CanRead => _stream.CanRead;
        public override bool CanSeek => _stream.CanSeek;
        public override bool CanWrite => _stream.CanWrite;
        public override long Length
        {
            get
            {
                lock (_lockObject)
                    return _stream.Length;
            }
        }

        public override void Flush()
        {
            lock (_lockObject)
                _stream.Flush();
        }
        public override void SetLength(long value)
        {
            lock (_lockObject)
                _stream.SetLength(value);
        }

        public override long Position
        {
            get
            {
                lock (_lockObject)
                    return _position;
            }
            set
            {
                lock (_lockObject)
                {
                    if (_position == value)
                        return;

                    // check is seekable
                    if (!CanSeek)
                        throw new NotSupportedException();

                    // set next offset
                    _position = value;
                }
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            lock (_lockObject)
            {
                var newPosition = origin switch
                {
                    SeekOrigin.Begin => offset,
                    SeekOrigin.Current => offset + _position,
                    SeekOrigin.End => offset + Length,
                    _ => throw new NotSupportedException(),
                };
                if (_position == newPosition)
                    return _position;

                // check is seekable
                if (!CanSeek)
                    throw new NotSupportedException();

                // set next offset
                _position = newPosition;
                return newPosition;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (_lockObject)
            {
                _stream.Position = _position;
                var ret = _stream.Read(buffer, offset, count);
                _position = _stream.Position;
                return ret;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (_lockObject)
            {
                _stream.Position = _position;
                _stream.Write(buffer, offset, count);
                _position = _stream.Position;
            }
        }
    }
}
