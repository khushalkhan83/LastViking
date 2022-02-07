using Game.Models;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UltimateSurvival;

public class FracturedStructureBehaviour : MonoBehaviour
{
    private const float FractureMaxDistance = 100f;
	private const float ViewDistanceUpdateRateMax = FractureMaxDistance + 1000f;

	private static readonly AnimationCurve ViewDistanceUpdateRate = new AnimationCurve(
		new Keyframe(FractureMaxDistance, 0),
		new Keyframe(ViewDistanceUpdateRateMax, 2f));

	private static Transform mPlayerTransform;

	#pragma warning disable 0649

	[SerializeField] private GameObject _fracturePrefab;
    [SerializeField] private GameObject _solidGroup;
    [SerializeField] private GameObject _brokenGroup;
	[SerializeField] private GameObject _depletedGroup;

	#pragma warning restore 0649

	private DisplayMode mDisplayMode;
	private float mTimeToViewDistanceSample;
	private FracturePack mFracturePack;
	
	public enum DisplayMode
	{
		None = 0,
		FOVSolid,
		FOVBroken,
		Fractured,
		Depleted,
	}
	
	public int totalPieces { get; private set; }

	public int totalBrokenPieces => fracturePack?.TotalBrokenPieces ?? 0;
    private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;
    private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;

    private Vector3 viewerPosition => PlayerEventHandler.transform.position;

	private GameObject solidGroup => _solidGroup;

	private GameObject depletedGroup => _depletedGroup;

    private GameObject brokenGroup => _brokenGroup;

	private bool isCompletelyBroken => totalBrokenPieces == totalPieces;

	private FracturePack fracturePack
	{
		get
		{
			return mFracturePack;
		}	

		set
		{
			if (mFracturePack == value)
				return;

			// Release last.
			if (mFracturePack != null)
				FracturePackProvider.Return(mFracturePack);

			mFracturePack = value;

			mFracturePack?.OnProvided(transform.parent);
		}
	}

	private bool isInsideFractureViewZone { get; set; }

    public DisplayMode displayMode
    {
        get
        {
            return mDisplayMode;
        }

		private set
        {
            if (mDisplayMode == value)
                return;

            mDisplayMode = value;

            solidGroup?.SetActive(value == DisplayMode.FOVSolid && value != DisplayMode.Depleted);

			if (brokenGroup != null)
				brokenGroup.SetActive(value == DisplayMode.FOVBroken && value != DisplayMode.Depleted);

			if (fracturePack != null)
				fracturePack.Active = value == DisplayMode.Fractured && value != DisplayMode.Depleted;

	        if (depletedGroup != null)
		        depletedGroup.SetActive(value == DisplayMode.Depleted || value == DisplayMode.Fractured);
        }
    }

    public void FracturePiece(Ray iRay)
    {
	    if (fracturePack == null)
	    {
		    fracturePack = FracturePackProvider.Get(_fracturePrefab);
            SendRenderers();
        }

        // Switch only if we are in viewable range, otherwise just set piece as broken.
        if (isInsideFractureViewZone)
        {
            displayMode = DisplayMode.Fractured;
        }

        // Raycast and Break pieces, if we hit one.
        var _hitPiece = fracturePack.RaycastPieces(iRay);

        if (_hitPiece == null)
            return;

        _hitPiece.Break(iRay);

        
    }

    public void FractureAll(Ray iRay)
    {
        if (isCompletelyBroken)
            return;

	    if (fracturePack == null)
	    {
		    fracturePack = FracturePackProvider.Get(_fracturePrefab);
            SendRenderers();
        }

        // Switch only if we are in viewable range, otherwise just set pieces as broken.
        if (isInsideFractureViewZone)
        {
            displayMode = DisplayMode.Fractured;
        }

        foreach (var _pieceBehaviour in fracturePack.Pieces.Where(_piece => _piece.state == FracturePieceBehaviour.State.Undisturbed))
        {
            _pieceBehaviour.Break(iRay);
        }
        
    }

    public void Reset()
    {
	    if (fracturePack != null)
	    {
		   fracturePack = null;
	    }
			
        displayMode = DisplayMode.FOVSolid;
    }

    private void OnEnable()
    {
        GameUpdateModel.OnUpdate += OnUpdate;
    }

    private void OnDisable()
    {
        GameUpdateModel.OnUpdate -= OnUpdate;
    }

    private void Awake()
    {
		// Pre-load fracture pack pool and get fracture pieces count for associated fracture pack.
		// Note: We need to store pieces count for logic that operates while we don't have fracture pack provided.
	    int _piecesCount;
	    FracturePackProvider.PreLoad(_fracturePrefab, out _piecesCount);
	    totalPieces = _piecesCount;

        Reset();
    }

    public void UpdateDisaplayModeAtStart() 
    {
        if (isCompletelyBroken)
        {
            displayMode = DisplayMode.Depleted;
        }
    }

    private void OnUpdate()
    {
	    UpdateFractureViewZone(Time.deltaTime, displayMode);
		    
        // Switch display modes.
        if (displayMode == DisplayMode.Fractured)
        {
            // Switch from Fractured display if we are too far away. 
            if (isInsideFractureViewZone == false)
            {
                // Decide how to represent fractured object outside of FractureMaxDistance.
                // Note: If we have enough unbroken pieces switch to FOV display, otherwise hide. So it's looks reasonable.
                if (totalBrokenPieces < totalPieces / 4)
                {
                    displayMode = DisplayMode.FOVSolid;
                }
                else if (totalBrokenPieces < totalPieces / 1.2f)
                {
                    displayMode = DisplayMode.FOVBroken;
                }
                else
                {
                    displayMode = DisplayMode.Depleted;
                }
            }

            // Wait until all pieces are hidden, then hide structure.
            if (isCompletelyBroken)
            {
                bool _allPiecesHidden = AllPiecesHidden();

	            if (_allPiecesHidden)
	            {
		            displayMode = DisplayMode.Depleted;

		            if (fracturePack != null)
		            {
			            fracturePack = null;
		            }
	            }
            }
        }
        else if (isInsideFractureViewZone)
        {
            // Switch back to Fractured, if we have broken pieces to show.
            // Note: In this implementation, If player completely breaks the object and then moves away to trigger
            // mode switch from Fractured he won't be able to switch back to Fractured to see the pieces disappearance animation.
            // If we are to implement that we can extend it here by adding check for non-hidden (still animating) pieces.
	        var _hasBrokenPieces = totalBrokenPieces > 0;

            if ((displayMode == DisplayMode.FOVSolid || displayMode == DisplayMode.FOVBroken) && _hasBrokenPieces)
            {
                displayMode = DisplayMode.Fractured;
            }
        }
    }

    private bool AllPiecesHidden()
    {
        //fracturePack.pieces.Any(_piece => _piece.state != FracturePieceBehaviour.State.Hidden) == false;
        for (int i = 0; i < fracturePack.Pieces.Length; i++)
        {
            if (fracturePack.Pieces.ElementAt(i).state != FracturePieceBehaviour.State.Hidden) return false;
        }
        return true;
    }

    private Vector2 structPos = new Vector2(), viewerPos = new Vector2();

	private void UpdateFractureViewZone(float iDeltaTime, DisplayMode iDisplayMode)
	{
		// Update Timer.
		mTimeToViewDistanceSample -= iDeltaTime;

		if (mTimeToViewDistanceSample > 0)
			return;

		// Sample distance.
		var _viewerPosition = viewerPosition;
        structPos.Set(transform.position.x, transform.position.z);
        viewerPos.Set(_viewerPosition.x, _viewerPosition.z);

        var _sampledDistance = (structPos - viewerPos).sqrMagnitude;
            //(new Vector2(transform.position.x, transform.position.z) - new Vector2(_viewerPosition.x, _viewerPosition.z)).sqrMagnitude;

		// Note: Addition to avoid rapid state switching in rare case of distance sampling noise.
		if (iDisplayMode == DisplayMode.Fractured)
		{
			_sampledDistance += 0.5f;
		}

		isInsideFractureViewZone = _sampledDistance <= FractureMaxDistance;

		// Set Timer.
		var _sampleTime = ViewDistanceUpdateRate.Evaluate(_sampledDistance);

		// Note: In case if we are sampling outside specified range, we want to randomize time a bit, so objects outside sampling range would have different update times.
		bool _randomizeTime = ViewDistanceUpdateRateMax < _sampledDistance;

		if (_randomizeTime)
			_sampleTime += UnityEngine.Random.Range(0f, 001f);

		mTimeToViewDistanceSample = _sampleTime;
	}

    System.Action<List<Renderer>> rendererUpdateListener;

    public void GetRenderersList(System.Action<List<Renderer>> setter)
    {
        rendererUpdateListener = setter;
        SendRenderers();
    }

    void SendRenderers()
    {
        if (rendererUpdateListener!=null)
        {
            List<Renderer> retVal = new List<Renderer>();
            retVal.AddRange(GetRendererFromLodGroup(solidGroup));
            retVal.AddRange(GetRendererFromLodGroup(brokenGroup));
            if (mFracturePack!=null)
            {
                foreach(MonoBehaviour mb in mFracturePack.Pieces)
                {
                    Renderer rrr = mb.gameObject.GetComponent<Renderer>();
                    if (rrr!=null)
                    {
                        retVal.Add(rrr);
                    }
                }   
            }
            rendererUpdateListener.Invoke(retVal);
        }
    }

    List< Renderer> GetRendererFromLodGroup(GameObject go)
    {
        List<Renderer> retVal = new List<Renderer>();
        if (go != null)
        {
            LODGroup group = go.GetComponent<LODGroup>();
            if (group != null)
            {
                if (group.lodCount > 0)
                {
                    retVal.AddRange(group.GetLODs()[0].renderers);
                }
            }
        }
        return retVal;
    }
}