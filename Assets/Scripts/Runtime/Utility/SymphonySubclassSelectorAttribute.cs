using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class SymphonySubclassSelectorAttribute : PropertyAttribute
{
	bool m_includeMono;

	public SymphonySubclassSelectorAttribute(bool includeMono = false)
	{
		m_includeMono = includeMono;
	}

	public bool IsIncludeMono()
	{
		return m_includeMono;
	}
}