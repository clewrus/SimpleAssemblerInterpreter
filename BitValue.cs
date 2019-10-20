using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS_Lab2 {
	public struct BitValue {
		public BitValue (int bitLength) {
			value = new bool[bitLength];
		}

		public BitValue (bool[] bitMask) : this(bitMask.Length) {
			Array.Copy(bitMask, this.value, bitMask.Length);
		}

		public BitValue (BitValue another) : this(another.value.Length) {
			Array.Copy(another.value, this.value, another.value.Length);
		}

		public BitValue (int bitLength, int value) : this(bitLength) {
			SetIntValue(value);
		}

		public bool SetIntValue (int number) {
			bool isPositive = (number >= 0);
			number = Math.Abs(number);

			for (int i = 0; i < value.Length; i++) {
				value[i] = (number % 2 == 1);
				number /= 2;
			}

			if (!isPositive) {
				Negate();
			}

			return number == 0;
		}

		public void Negate () {
			for (int i = 0; i < value.Length; i++) {
				value[i] = !value[i];
			}

			var one = new bool[value.Length];
			one[0] = true;

			this.Add(new BitValue(one));
		}

		public bool Add (BitValue right) {
			if (this.value.Length != right.value.Length) {
				throw new Exception("Bit values must be equal length");
			}

			bool overflow = false;
			for (int i = 0; i < value.Length; i++) {
				bool initValue = value[i];
				value[i] =  overflow ^ (value[i] ^ right.value[i]);
				overflow = initValue & right.value[i] | overflow & (initValue | right.value[i]);
			}

			return overflow;
		}

		public int ToInt () {
			var cpy = new BitValue(this);
			bool negative = cpy.IsNegative;

			if (negative) {
				cpy.Negate();
			}

			int result = 0;
			for (int i = cpy.value.Length - 1; i >= 0; i--) {
				result *= 2;
				result += (cpy.value[i]) ? 1 : 0;
			}

			return result * ((negative) ? -1 : 1);
		}

		public override String ToString () {
			return ToInt().ToString();
		}

		public bool[] GetValue () {
			var copedValue = new bool[value.Length];
			Array.Copy(value, copedValue, value.Length);
			return copedValue;
		}

		public bool IsNegative { get => value[value.Length - 1]; }

		private bool[] value; 
	}
}
