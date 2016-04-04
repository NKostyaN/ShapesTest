using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tasks : MonoBehaviour
{
	public SpriteRenderer TaskSprite;
	public Sprite[] shapesHD, shapesLD;

	[HideInInspector]
	public List<Vector2> tasksPoints = new List<Vector2>();
	[HideInInspector]
	public float taskBound;

	private int currShape;
	private Color[] pixColors;

	void Start ()
	{
		NextTask();	
	}
	

	public void NextTask ()
	{
		tasksPoints.Clear();
		int r = Random.Range(0, shapesLD.Length);
		while (r == currShape)
		{
			r = Random.Range(0, shapesLD.Length);
		}
		currShape = r;
		int x = Mathf.FloorToInt(shapesLD[currShape].rect.x);
		int y = Mathf.FloorToInt(shapesLD[currShape].rect.y);
		int w = Mathf.FloorToInt(shapesLD[currShape].rect.width);
		int h = Mathf.FloorToInt(shapesLD[currShape].rect.height);
		pixColors = shapesLD[currShape].texture.GetPixels(x, y, w, h);
		TaskSprite.sprite = shapesHD[currShape];

		for (int i = 0; i < h; i++)
			for (int j = 0; j < h; j++)
				if (pixColors[j+(h*i)] != Color.black)
					tasksPoints.Add(new Vector2(j,i));
		
		tasksPoints.Sort(new Vector2Comparer());
		Vector2 delta = tasksPoints[0];
		for (int i = 0; i < tasksPoints.Count; i++)
		{
			tasksPoints[i] = tasksPoints[i]-delta;
		}
		taskBound = Vector2.Distance(tasksPoints[0], tasksPoints[tasksPoints.Count-1]);
	}
}
