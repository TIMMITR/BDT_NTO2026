using Scellecs.Morpeh;

namespace Features.UnitPack.Requests
{
    public struct ChangeMoveDataRequest : IRequestData
    {
        public int dataIndex;

        public ChangeMoveDataRequest(int dataIndex)
        {
            this.dataIndex = dataIndex;
        }
    }
}
