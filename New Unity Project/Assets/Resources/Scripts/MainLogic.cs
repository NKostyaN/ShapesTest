using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainLogic : MonoBehaviour
{
	public ParticleSystem drawFX;
	public Text txt, scoreTxt, finScore, timeText;
	public Image timeLine;
	public Color startColor, endColor;
	public GameObject gameOverBTN;
	public int score = 0;
	public float step = 0.01f;
	public float threshold = 0.3f;
	public float pMult = 50;
	public float timeToSolve = 30;

	private bool gameOver = true;
	public List<Vector2> drawedPoints = new List<Vector2>(), simplerPoints = new List<Vector2>();
	private float dist = 0;
	private float drawBound = 0;
	private float t = 0;
	private int find = 0;
	private Vector2 m_pos, lastPos;
	private ParticleSystem.EmissionModule em;

	void Start ()
	{
		score = 0;
		em = drawFX.emission;
		timeText.text = "Time left: " + timeToSolve;
	}

	void Update ()
	{
		if (!gameOver)
		{		
			timeLine.fillAmount = timeLine.fillAmount - Time.deltaTime / timeToSolve;
			timeLine.color = Color.Lerp(startColor, endColor, t);
			t += Time.deltaTime / timeToSolve;
			if (timeLine.fillAmount <= 0)
			{
				GameOver();
			}

			Vector3 v =  Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (v.x <= 0)
				return;
			m_pos = new Vector2 (v.x, v.z);
			if (drawFX != null)
				drawFX.gameObject.transform.position = new Vector3(m_pos.x, 1, m_pos.y);
				
			if (Input.GetMouseButtonDown(0))
			{
				drawedPoints.Clear();
				drawFX.gameObject.SetActive(true);
				lastPos = m_pos;
			}

			if (Input.GetMouseButton(0))
			{
				dist = Vector2.Distance(lastPos, m_pos);
				if(dist > step)
				{
					em.rate = new ParticleSystem.MinMaxCurve(dist*pMult);
					lastPos = m_pos;
					drawedPoints.Add(m_pos);
				}
			}

			if (Input.GetMouseButtonUp(0))
			{
				drawFX.gameObject.SetActive(false);
				if(drawedPoints.Count > 1)
					CheckShape();
			}
		}
	}

	void CheckShape ()
	{
		find = 0;
		simplerPoints.Clear();
		drawedPoints.Sort(new Vector2Comparer());
		Vector2 delta = drawedPoints[0];
		for (int i = 0; i < drawedPoints.Count; i++)
		{
			drawedPoints[i] = drawedPoints[i]-delta;
		}

		drawBound = Vector2.Distance(drawedPoints[0], drawedPoints[drawedPoints.Count-1]);
		for (int i = 0; i < drawedPoints.Count; i++)
		{
			float ratio = GetComponent<Tasks>().taskBound / drawBound;
			drawedPoints[i] =  new Vector2(Mathf.FloorToInt(drawedPoints[i].x * ratio), Mathf.FloorToInt(drawedPoints[i].y * ratio));	
		}

		simplerPoints.Add(drawedPoints[0]);
		for (int i = 1; i < drawedPoints.Count; i++)
		{
			if (simplerPoints[simplerPoints.Count - 1] != drawedPoints[i])
				simplerPoints.Add(drawedPoints[i]);
		}

		for (int i = 0; i < simplerPoints.Count; i++)
		{
			if(GetComponent<Tasks>().tasksPoints.Contains(simplerPoints[i]))
				find++;
		}
		if ((float)find/GetComponent<Tasks>().tasksPoints.Count >= threshold)
		{
//			Debug.Log("Done");
			nextRound();
		}
		else
		{
//			Debug.Log("No");
			txt.color = Color.red;
			txt.text = "Nope...";
			Invoke("SetText", 0.5f);
		}
	}

	void GameOver()
	{
		gameOverBTN.SetActive(true);
		gameOver = true;
		txt.color = Color.red;
		txt.text = "Game over";
	}

	void SetText()
	{
		txt.color = Color.black;
		txt.text = "Draw";
	}

	void nextRound()
	{
		score++;
		t = 0;
		timeLine.fillAmount = 1;
		timeLine.color = startColor;
		timeToSolve = timeToSolve - 0.5f;
		timeText.text = "Time left: " + timeToSolve;
		scoreTxt.text = "Score: " + score;
		finScore.text = "Your score is: " + score;
		txt.color = Color.blue;
		txt.text = "Done!";
		Invoke("SetText", 0.5f);
		GetComponent<Tasks>().NextTask();
	}

	public void NewGame()
	{
		gameOver = false;
		GetComponent<Tasks>().NextTask();
	}

	public void ReloadLevel ()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
