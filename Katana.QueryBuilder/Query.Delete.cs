namespace Katana
{
    public partial class Query
    {
        public Query AsDelete()
        {
            Method = "delete";
            return this;
        }

    }
}
