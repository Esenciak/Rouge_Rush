using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CinemachineTargetGroup))]
// kod z asseta

public class CinemachineTarget : MonoBehaviour
{
	private CinemachineTargetGroup cinemachineTargetGroup;

	#region Tooltip
	[Tooltip("Populate with the CursorTarget gameobject")]
	#endregion Tooltip
	[SerializeField] private Transform cursorTarget;



	private void Awake()
	{
		// �aduje components
		cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
	}

	
	void Start()
	{
		SetCinemachineTargetGroup();
	}

	
	private void SetCinemachineTargetGroup()
	{
		
		CinemachineTargetGroup.Target cinemachineGroupTarget_player = new CinemachineTargetGroup.Target { weight = 1f, radius = 2.5f, target = GameManager.Instance.GetPlayer().transform };

		CinemachineTargetGroup.Target cinemachineGroupTarget_cursor = new CinemachineTargetGroup.Target { weight = 1f, radius = 1f, target = cursorTarget };

		CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[] { cinemachineGroupTarget_player, cinemachineGroupTarget_cursor };

		cinemachineTargetGroup.m_Targets = cinemachineTargetArray;

	}

	private void Update()
	{
		cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
	}

}
