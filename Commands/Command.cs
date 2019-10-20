using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS_Lab2 {
	public abstract class Command {
		public Command (ProcessorModel parent) {
			this.Parent = parent;
		}

		public void UpdateCommand (AssemblerCommand command) {
			if (command.Name != CommandName) {
				throw new Exception("WrongCommandExecuted");
			}

			IsDone = false;
			currentCommand = command;

			executionSteps = null;
			executionSteps = ChoseExecutionSteps();
		}

		protected abstract List<Action> ChoseExecutionSteps ();

		public void MakeStep () {
			BitValue curStep = Parent.registers[RegisterName.TC].GetValue();

			if (curStep.ToInt() >= executionSteps.Count) {
				IsDone = true;
				return;
			}
			executionSteps[curStep.ToInt()].Invoke();

			var one = new bool[ProcessorModel.BIT_LENGTH];
			one[0] = true;

			curStep.Add(new BitValue(one));
			Parent.registers[RegisterName.TC].SetValue(curStep);

			IsDone = (curStep.ToInt() >= executionSteps.Count);
		}

		public abstract AssemblerCommand.OpCode CommandName { get; }

		public ProcessorModel Parent { get; private set; }
		public bool IsDone { get; private set; }

		private List<Action> executionSteps;

		protected AssemblerCommand currentCommand;
		protected AssemblerCommand.ICommandOpRand[] currentOpRands {
			get => currentCommand.commandOpRands.ToArray();
		}
	}
}
