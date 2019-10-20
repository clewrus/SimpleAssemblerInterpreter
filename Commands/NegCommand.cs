using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS_Lab2 {
	public class NegCommand : Command {
		public NegCommand (ProcessorModel parent) : base(parent) { }
		public override AssemblerCommand.OpCode CommandName => AssemblerCommand.OpCode.Neg;

		protected override List<Action> ChoseExecutionSteps () {
			var opRands = currentOpRands;

			if (opRands.Length < 1) {
				throw new Exception("Neg command must have one arguments");
			}

			if (!(opRands[0] is AssemblerCommand.RegName)) {
				throw new Exception("First argument of neg must be register");
			}

			List<Action> stageList = new List<Action>();
			stageList.Add(MakeNegation);

			return stageList;
		}

		private void MakeNegation () {
			RegisterName destReg = ((AssemblerCommand.RegName)currentOpRands[0]).Value;

			BitValue value = new BitValue(Parent.registers[destReg].GetValue());
			value.Negate();

			Parent.registers[destReg].SetValue(value);
			((StatusRegister)Parent.registers[RegisterName.PS]).SetValue(value.IsNegative);
		}
	}
}
