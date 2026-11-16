using Scellecs.Morpeh;

namespace Features.UnitPack.Requests
{
    public struct ChangeMoveAnimationRequest : IRequestData
    {
        public int dataIndex;

        public ChangeMoveAnimationRequest(int dataIndex)
        {
            this.dataIndex = dataIndex;
        }
    }
}