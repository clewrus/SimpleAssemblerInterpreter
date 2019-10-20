using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS_Lab2 {
	public class AddCommand : Command {
		public AddCommand (ProcessorModel parent) : base(parent) { }
		public override AssemblerCommand.OpCode CommandName => AssemblerCommand.OpCode.Add;

		protected override List<Action> ChoseExecutionSteps () {
			var opRands = currentOpRands;

			if (opRands.Length < 2) {
				throw new Exception("Add command must have two arguments");
			}

			if (!(opRands[0] is AssemblerCommand.RegName)) {
				throw new Exception("First argument of add must be register");
			}

			List<Action> stageList = new List<Action>();

			if (opRands[1] is AssemblerCommand.ConstValue) {
				stageList.Add(AddingWithConstValue);
			} else {
				stageList.Add(AddingWithRegister);
			}

			return stageList;
		}

		private void AddingWithConstValue () {
			RegisterName destReg = ((AssemblerCommand.RegName)currentOpRands[0]).Value;
			BitValue value = ((AssemblerCommand.ConstValue)currentOpRands[1]).Value;

			var curDestValue = Parent.registers[destReg].GetValue();
			curDestValue.Add(value);

			((StatusRegister)Parent.registers[RegisterName.PS]).SetValue(curDestValue.IsNegative);

			Parent.registers[destReg].SetValue(curDestValue);
		}

		private void AddingWithRegister () {
			RegisterName destReg = ((AssemblerCommand.RegName)currentOpRands[0]).Value;
			RegisterName sourceReg = ((AssemblerCommand.RegName)currentOpRands[1]).Value;

			var curDestValue = Parent.registers[destReg].GetValue();
			curDestValue.Add(Parent.registers[sourceReg].GetValue());

			((StatusRegister)Parent.registers[RegisterName.PS]).SetValue(curDestValue.IsNegative);

			Parent.registers[destReg].SetValue(curDestValue);
		}
	}
}
