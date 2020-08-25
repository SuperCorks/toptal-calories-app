using System.Text;

namespace Calories.App.Adapters
{
    public class StringValueStore : GenericValueStore<string>
    {
        protected override string Deserialize(byte[] bytes)
        {
            if (bytes == null) return null;

            return Encoding.UTF8.GetString(bytes);
        }

        protected override byte[] Serialize(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }
    }
}
