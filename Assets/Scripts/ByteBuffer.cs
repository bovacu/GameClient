using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Reflection;

public class ByteBuffer {
    private List<byte>  buffer;
    private byte[]      readBuffer;
    private int         readPosition;
    private bool        bufferUpdated = false;

    public ByteBuffer() {
        this.buffer = new List<byte>();
        this.readPosition = 0;
    }

    public int getreadPosition() {
        return this.readPosition;
    }
    public byte[] toArray() { 
        return this.buffer.ToArray();
    }
    public int count() {
        return this.buffer.Count;
    }
    public int length() {
        return this.count() - this.readPosition;
    }
    public void clear() {
        this.buffer.Clear();
        this.readPosition = 0;
    }

	public void writeByte(byte input) {
		this.buffer.Add(input);
		this.bufferUpdated = true;
	}
	public void writeBytes(byte[] input) {
		this.buffer.AddRange(input);
		this.bufferUpdated = true;
	}
	public void writeShort(short input) {
		this.buffer.AddRange(BitConverter.GetBytes(input));
		this.bufferUpdated = true;
	}
	public void writeInteger(int input) {
		this.buffer.AddRange(BitConverter.GetBytes(input));
		this.bufferUpdated = true;
	}
	public void writeLong(long input) {
		this.buffer.AddRange(BitConverter.GetBytes(input));
		this.bufferUpdated = true;
	}
	public void writeFloat(float input) {
		this.buffer.AddRange(BitConverter.GetBytes(input));
		this.bufferUpdated = true;
	}
	public void writeString(string input) {
		this.buffer.AddRange(BitConverter.GetBytes(input.Length));
		this.buffer.AddRange(Encoding.ASCII.GetBytes(input));
		this.bufferUpdated = true;
	}
	public void writeVector3(Vector3 input) {
		byte[] vectorArray = new byte[sizeof(float) * 3];

		Buffer.BlockCopy(BitConverter.GetBytes(input.x), 0, vectorArray, 0 * sizeof(float), sizeof(float));
		Buffer.BlockCopy(BitConverter.GetBytes(input.y), 1, vectorArray, 0 * sizeof(float), sizeof(float));
		Buffer.BlockCopy(BitConverter.GetBytes(input.z), 2, vectorArray, 0 * sizeof(float), sizeof(float));

		this.buffer.AddRange(vectorArray);
		this.bufferUpdated = true;
	}
	public void writeQuanternion(Quaternion input) {
		byte[] vectorArray = new byte[sizeof(float) * 4];

		Buffer.BlockCopy(BitConverter.GetBytes(input.x), 0, vectorArray, 0 * sizeof(float), sizeof(float));
		Buffer.BlockCopy(BitConverter.GetBytes(input.y), 1, vectorArray, 0 * sizeof(float), sizeof(float));
		Buffer.BlockCopy(BitConverter.GetBytes(input.z), 2, vectorArray, 0 * sizeof(float), sizeof(float));
		Buffer.BlockCopy(BitConverter.GetBytes(input.w), 3, vectorArray, 0 * sizeof(float), sizeof(float));

		this.buffer.AddRange(vectorArray);
		this.bufferUpdated = true;
	}



	public byte readByte(bool _peek = true) {
		if (this.buffer.Count > this.readPosition) {
			if (this.bufferUpdated) {
				this.readBuffer = this.buffer.ToArray();
				this.bufferUpdated = false;
			}

			byte value = this.readBuffer[this.readPosition];

			if (_peek & this.buffer.Count > this.readPosition) {
				this.readPosition += 1;
			}

			return value;
		} else {
			throw new Exception("[BYTE] Error");
		}
	}
	public byte[] readBytes(int Length, bool _peek = true) {
		if (this.buffer.Count > this.readPosition) {
			if (this.bufferUpdated) {
				this.readBuffer = this.buffer.ToArray();
				this.bufferUpdated = false;
			}

			byte[] value = this.buffer.GetRange(this.readPosition, Length).ToArray();

			if (_peek) {
				this.readPosition += Length;
			}

			return value;
		} else {
			throw new Exception("[BYTE[]] Error");
		}
	}
	public short readShort(bool _peek = true) {
		if (this.buffer.Count > this.readPosition) {
			if (this.bufferUpdated) {
				this.readBuffer = this.buffer.ToArray();
				this.bufferUpdated = false;
			}

			short value = BitConverter.ToInt16(this.readBuffer, this.readPosition);

			if (_peek & this.buffer.Count > this.readPosition) {
				this.readPosition += 2;
			}

			return value;
		} else {
			throw new Exception("[SHORT] Error");
		}
	}
	public int readInteger(bool _peek = true) {
		if (this.buffer.Count > this.readPosition) {
			if (this.bufferUpdated) {
				this.readBuffer = this.buffer.ToArray();
				this.bufferUpdated = false;
			}

			int value = BitConverter.ToInt32(this.readBuffer, this.readPosition);

			if (_peek & this.buffer.Count > this.readPosition) {
				this.readPosition += 4;
			}

			return value;
		} else {
			throw new Exception("[INT] Error");
		}
	}
	public long readLong(bool _peek = true) {
		if (this.buffer.Count > this.readPosition) {
			if (this.bufferUpdated) {
				this.readBuffer = this.buffer.ToArray();
				this.bufferUpdated = false;
			}

			long value = BitConverter.ToInt64(this.readBuffer, this.readPosition);

			if (_peek & this.buffer.Count > this.readPosition) {
				this.readPosition += 8;
			}

			return value;
		} else {
			throw new Exception("[LONG] Error");
		}
	}
	public float readFloat(bool _peek = true) {
		if (this.buffer.Count > this.readPosition) {
			if (this.bufferUpdated) {
				this.readBuffer = this.buffer.ToArray();
				this.bufferUpdated = false;
			}

			float value = BitConverter.ToSingle(this.readBuffer, this.readPosition);

			if (_peek & this.buffer.Count > this.readPosition) {
				this.readPosition += 4;
			}

			return value;
		} else {
			throw new Exception("[FLOAT] Error");
		}
	}
	public string readString(bool _peek = true) {
		int length = readInteger(true);

		if (this.bufferUpdated) {
			this.readBuffer = this.buffer.ToArray();
			this.bufferUpdated = false;
		}

		string value = Encoding.ASCII.GetString(this.readBuffer, this.readPosition, length);

		if (_peek & this.buffer.Count > this.readPosition) {
			this.readPosition += length;
		}

		return value;
	}
	public Vector3 readVector3(bool _peek = true) {
		if (this.bufferUpdated) {
			this.readBuffer = this.buffer.ToArray();
			this.bufferUpdated = false;
		}

		byte[] value = this.buffer.GetRange(this.readPosition, sizeof(float) * 3).ToArray();
		Vector3 vector3;
		vector3.x = BitConverter.ToSingle(value, 0 * sizeof(float));
		vector3.y = BitConverter.ToSingle(value, 1 * sizeof(float));
		vector3.z = BitConverter.ToSingle(value, 2 * sizeof(float));

		if (_peek) {
			this.readPosition += sizeof(float) * 3;
		}

		return vector3;
	}
	public Quaternion readQuaternion(bool _peek = true) {
		if (this.bufferUpdated) {
			this.readBuffer = this.buffer.ToArray();
			this.bufferUpdated = false;
		}

		byte[] value = this.buffer.GetRange(this.readPosition, sizeof(float) * 4).ToArray();
		Quaternion quaternion;
		quaternion.x = BitConverter.ToSingle(value, 0 * sizeof(float));
		quaternion.y = BitConverter.ToSingle(value, 1 * sizeof(float));
		quaternion.z = BitConverter.ToSingle(value, 2 * sizeof(float));
		quaternion.w = BitConverter.ToSingle(value, 3 * sizeof(float));

		if (_peek) {
			this.readPosition += sizeof(float) * 4;
		}

		return quaternion;
	}

    protected virtual void dispose(bool disposing) {
		if (disposing) {
			this.buffer.Clear();
			this.readPosition = 0;
		}
	}

    #region IDisposable implementation

    public void dispose() {
        dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
