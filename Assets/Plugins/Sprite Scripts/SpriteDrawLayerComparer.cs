using System.Collections.Generic;

public class SpriteDrawLayerComparer : IComparer<SpriteMesh_Managed>
{
	public int Compare(SpriteMesh_Managed a, SpriteMesh_Managed b)
	{
		if (a.drawLayer > b.drawLayer)
		{
			return 1;
		}
		if (a.drawLayer < b.drawLayer)
		{
			return -1;
		}
		return 0;
	}
}
