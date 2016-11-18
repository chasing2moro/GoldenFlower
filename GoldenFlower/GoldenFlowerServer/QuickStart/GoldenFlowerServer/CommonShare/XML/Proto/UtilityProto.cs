using System.Collections;
using System.Collections.Generic;

public class UtilityProto
{
    //public List<RecordRecipe> m_RecordRecipeList;


    // Use this for initialization
    public static void CacheAllRecord () {
        XMLReader.CacheRecord<int, RecordRecipe>(RecordRecipe.GetConfigPath());
        XMLReader.CacheRecord<int, RecordRecipe>("recipe1");
    }

    public static VALUE GetRecord<KEY, VALUE>(string vPath, KEY vKey) where VALUE : XMLRecord, new()
    {
        return XMLReader.GetRecord<KEY, VALUE>(vPath, vKey);
    }

    /*
    public RecordRecipe recordRecipe1;
    public RecordRecipe recordRecipe2;
    public void Read()
    {
        recordRecipe1 = XMLReader.GetRecord<int, RecordRecipe>(RecordRecipe.GetConfigPath(), 2);
        recordRecipe2 = XMLReader.GetRecord<int, RecordRecipe>("recipe1", 5);
    }
    */

}
