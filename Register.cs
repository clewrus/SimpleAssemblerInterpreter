using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS_Lab2 {
	public enum RegisterName { None, IR, PC, TC, PS, R1, R2, R3, R4 }

	public abstract class Register {
		public Register (ProcessorModel parent) {
			this.Parent = parent;
			value = new BitValue(new bool[ProcessorModel.BIT_LENGTH]);
		}

		public virtual BitValue GetValue () {
			return value;
		}

		public virtual void SetValue (BitValue value) {
			this.value = value;
		}

		protected BitValue value;
		public ProcessorModel Parent { get; private set; }
	}

	public class StatusRegister : Register {
		public StatusRegister (ProcessorModel parent) : base(parent) { 	}

		public override String ToString () {
			return (IsNegative()) ? "-" : "+";
		}

		public void SetValue (bool negative) {
			SetValue(new BitValue(ProcessorModel.BIT_LENGTH, (negative)? 1 : 0));
		}

		public bool IsNegative () {
			return this.value.ToInt() != 0;
		}
	}

	public class ValueRegister : Register {
		public ValueRegister (ProcessorModel parent) : base(parent) { }

		public override String ToString () {
			string resultString = "";
			bool[] bits = this.value.GetValue();

			for (int i = bits.Length - 1; i >= 0; --i) {
				resultString = $"{resultString} {((bits[i]) ? 1 : 0)} |";
			}

			return resultString;
		}
	}

	public class CommandRegister : Register {
		public CommandRegister (ProcessorModel parent) : base(parent) { }

		public override String ToString () {
			return curCommand.ToString();
		}

		public void SetValue (AssemblerCommand command) {
			curCommand = command;
		}

		public AssemblerCommand GetCommand () {
			return curCommand;
		}

		private AssemblerCommand curCommand;
	}
}
