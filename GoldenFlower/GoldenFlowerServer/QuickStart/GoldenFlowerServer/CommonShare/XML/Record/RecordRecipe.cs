using System.Collections;


public enum RecipeType{
	Meat,
	Vegetable,
	Dessert
}

[System.Serializable]
public class RecordRecipe : XMLRecord {
  

     //子类要实现它
    public static string GetConfigPath()
    {
        return "recipe";
    }


    [XMLAttributeInt]
    public int id;

    [XMLAttributeString]
	public string recipename;

	[XMLAttributeInt]
	public int preptime;

	[XMLAttributeEnum(typeof(RecipeType))]
	public RecipeType type;
}
