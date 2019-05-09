using System;

namespace NodeGraphs.Components
{
    public class GraphConnection : GraphComponent
    {
        private NodeJoint outputJoint;
        private NodeJoint inputJoint;

        public NodeJoint OutputJoint
        {
            get => outputJoint;
            set
            {
                if (value == outputJoint)
                    return;
                outputJoint = value;
                OnPropertyChanged(nameof(OutputJoint));
            }
        }
        public NodeJoint InputJoint
        {
            get => inputJoint;
            set
            {
                if (value == inputJoint)
                    return;
                inputJoint = value;
                OnPropertyChanged(nameof(InputJoint));
            }
        }

        public GraphConnection(string id) : base(id)
        {

        }

        /// <summary>
        /// Connects the output joint to the input joint
        /// </summary>
        /// <param name="secondJoint"></param>
        /// <param name="firstJoint"></param>
        public void Connect(NodeJoint firstJoint, NodeJoint secondJoint)
        {
            if (firstJoint == null)
                throw new ArgumentNullException(nameof(firstJoint));
            if (secondJoint == null)
                throw new ArgumentNullException(nameof(secondJoint));
            if (!firstJoint.CanConnect || !secondJoint.CanConnect)
                return;

            JointRole fRole = firstJoint.Role;
            JointRole sRole = secondJoint.Role;

            bool canConnect = (fRole == sRole) && fRole == JointRole.None;
            canConnect |= fRole == JointRole.Input && sRole == JointRole.Output;
            canConnect |= fRole == JointRole.Output && sRole == JointRole.Input;
            canConnect |= fRole == JointRole.Output && sRole == JointRole.None;
            canConnect |= fRole == JointRole.None && sRole == JointRole.Output;

            if (!canConnect)
                return;

            if (fRole == JointRole.Input)
                InputJoint = firstJoint;
            else
                InputJoint = secondJoint;
            if (sRole == JointRole.Output)
                OutputJoint = secondJoint;
            else
                OutputJoint = firstJoint;

            InputJoint.AddConnection(this);
            OutputJoint.AddConnection(this);
        }
    }
}