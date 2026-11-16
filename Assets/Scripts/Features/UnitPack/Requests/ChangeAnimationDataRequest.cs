using Scellecs.Morpeh;

namespace Features.UnitPack.Requests
{
    public struct ChangeAnimationDataRequest : IRequestData
    {
        public int dataIndex;

        public ChangeAnimationDataRequest(int dataIndex)
        {
            this.dataIndex = dataIndex;
        }
    }
}