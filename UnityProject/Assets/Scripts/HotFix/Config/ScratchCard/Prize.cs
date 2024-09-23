
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using Newtonsoft.Json.Linq;



namespace cfg.ScratchCard
{
/// <summary>
/// 刮刮卡奖励
/// </summary>

public sealed partial class Prize : Luban.BeanBase
{
    public Prize(JToken _buf) 
    {
        JObject _obj = _buf as JObject;
        { var __json0 = _obj.GetValue("prizeItems"); PrizeItems = new System.Collections.Generic.List<ScratchCard.PrizeItem>((__json0 as JArray).Count); foreach(JToken __e0 in __json0) { ScratchCard.PrizeItem __v0;  __v0 = ScratchCard.PrizeItem.DeserializePrizeItem(__e0);  PrizeItems.Add(__v0); }   }
        Weight = (int)_obj.GetValue("weight");
    }

    public static Prize DeserializePrize(JToken _buf)
    {
        return new ScratchCard.Prize(_buf);
    }

    /// <summary>
    /// 奖励
    /// </summary>
    public readonly System.Collections.Generic.List<ScratchCard.PrizeItem> PrizeItems;
    /// <summary>
    /// 权重
    /// </summary>
    public readonly int Weight;


    public const int __ID__ = -508789234;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
        foreach (var _e in PrizeItems) { _e?.ResolveRef(tables); }
    }

    public override string ToString()
    {
        return "{ "
        + "prizeItems:" + Luban.StringUtil.CollectionToString(PrizeItems) + ","
        + "weight:" + Weight + ","
        + "}";
    }
}
}
