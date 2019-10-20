using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS_Lab2 {
	public class MovCommand : Command {
		public MovCommand (ProcessorModel parent) : base(parent) { }
		public override AssemblerCommand.OpCode CommandName => AssemblerCommand.OpCode.Mov;

		protected override List<Action> ChoseExecutionSteps () {
			var opRands = currentOpRands;

			if (opRands.Length < 2) {
				throw new Exception("Mov command must have two arguments");
			}

			if (!(opRands[0] is AssemblerCommand.RegName)) {
				throw new Exception("First argument of move must be register");
			}

			List<Action> stageList = new List<Action>();

			if (opRands[1] is AssemblerCommand.ConstValue) {
				stageList.Add(MovingFromConstValue);
			} else {
				stageList.Add(MovingFromRegister); 
			}

			return stageList;
		}

		private void MovingFromConstValue () {
			RegisterName destReg = ((AssemblerCommand.RegName)currentOpRands[0]).Value;
			BitValue value = ((AssemblerCommand.ConstValue)currentOpRands[1]).Value;

			((StatusRegister)Parent.registers[RegisterName.PS]).SetValue(value.IsNegative);

			Parent.registers[destReg].SetValue(value);
		}

		private void MovingFromRegister () {
			RegisterName destReg = ((AssemblerCommand.RegName)currentOpRands[0]).Value;
			RegisterName sourceReg = ((AssemblerCommand.RegName)currentOpRands[1]).Value;

			((StatusRegister)Parent.registers[RegisterName.PS]).SetValue(Parent.registers[sourceReg].GetValue().IsNegative);

			Parent.registers[destReg].SetValue(new BitValue(Parent.registers[sourceReg].GetValue()));
		}
	}
}
