
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using Newtonsoft.Json.Linq;



namespace cfg
{

public partial struct vector2
{
    public vector2(JToken _buf) 
    {
        JObject _obj = _buf as JObject;
        X = (float)_obj.GetValue("x");
        Y = (float)_obj.GetValue("y");
    }

    public static vector2 Deserializevector2(JToken _buf)
    {
        return new vector2(_buf);
    }

    public readonly float X;
    public readonly float Y;



    public  void ResolveRef(Tables tables)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "x:" + X + ","
        + "y:" + Y + ","
        + "}";
    }
}
}
