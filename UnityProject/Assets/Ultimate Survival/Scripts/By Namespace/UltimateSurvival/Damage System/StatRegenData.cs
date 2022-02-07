﻿using UnityEngine;

namespace UltimateSurvival
{
    [System.Serializable]
	public class StatRegenData
	{
		public bool CanRegenerate { get { return m_Enabled && !IsPaused; } }

		public bool Enabled { get { return m_Enabled; } }

		public bool IsPaused { get { return Time.time < m_NextRegenTime; } }

		public float RegenDelta { get { return m_Speed * Time.deltaTime; } }

		[SerializeField]
		private bool m_Enabled = true;

		[SerializeField]
		private float m_Pause = 2f;

		[SerializeField]
		[Clamp(0f, 1000f)]
		private float m_Speed = 10f;

		private float m_NextRegenTime;


		public void Pause()
		{
			m_NextRegenTime = Time.time + m_Pause;
		}
	}
}