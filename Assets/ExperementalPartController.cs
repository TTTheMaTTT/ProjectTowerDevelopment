﻿using UnityEngine;
using System.Collections;

public class ExperementalPartController : MonoBehaviour {

	
	public int numb, type, mod, animationMod, frame, addictiveFrame;
	public GAF.Core.GAFMovieClip mov;
	private int realNumb;
	
	public string currentState, nextState;
	public bool loop;
	public bool reload;
	public bool led;
	public int FPS;
	public int right;
	public float isItRight;
	public PartConroller[] parts;
	
	public int partsNumb;
	public SATClass.sat[] sats;
	
	private AnimationInterpretator interp;
	private SpFunctions sp;
	private SoundManager sManager;
	public AudioSource efxSource;
	private uint k = 1;
	
	public int jj;
	
	public void Awake () 
	{
		partsNumb = 0;
		interp = GetComponent<AnimationInterpretator> ();	
		sp = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<SpFunctions> ();
		sManager=GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<SoundManager> ();
	}
	
	public void Update()
	{
		isItRight = right*Mathf.Sign(transform.lossyScale.x);
		
		if (gameObject.GetComponent<WeaponClass> () == null) 
			realNumb = numb;
		//nextState = interp.animms [type].anims [numb].rsequence;
		else 
		{
			realNumb= gameObject.GetComponent<WeaponClass>().active==false? 
				numb: realNumb=interp.animms[type].anims.Length-numb-1;
			if (gameObject.GetComponent<WeaponClass>().active)
				jj++;
			/*interp.animms [type].anims [interp.animms [type].anims.Length-numb-1].rsequence:
					interp.animms[type].anims[numb].rsequence;*/
		}
		if (isItRight >= 0)
			nextState = interp.animms [type].anims [realNumb].rsequence;
		else
			nextState = interp.animms [type].anims [realNumb].lsequence;
		for (int i=0; i<parts.Length; i++)
			if (parts [i].gameObject.GetComponent<WeaponClass> () != null)
				if ((parts [i].gameObject.GetComponent<WeaponClass> ().orientation == isItRight) &&
				    (parts [i].gameObject.GetComponent<WeaponClass> ().active))
					nextState = parts [i].nextState;
		if ((nextState!=currentState)&&(nextState!="StopAnimation"))
		{	
			currentState=nextState;
			loop=interp.animms[type].anims[numb].loop;
			FPS=interp.animms[type].anims[numb].FPS;
			sats=interp.animms [type].anims [numb].rsats;
			mov.setSequence(currentState,true);
			mov.settings.targetFPS=k*(uint)FPS;
			if (loop)
				mov.settings.wrapMode=GAF.Core.GAFWrapMode.Loop;
			else
				mov.settings.wrapMode=GAF.Core.GAFWrapMode.Once;
			mov.setPlaying(true);
		}
		if (interp.animms [type].anims [numb].stepByStep)
			mov.setPlaying(true);
		if (interp.animms [type].anims [numb].stopStepByStep)
			mov.gotoAndStop(mov.getCurrentFrameNumber());
		if (!led) 
			mov.setPlaying (true);
		else 
			mov.setPlaying(false);
		frame = (int)mov.getCurrentFrameNumber ();
		if (addictiveFrame>-1)
		{
			mov.gotoAndStop(mov.currentSequence.startFrame+k*(uint)addictiveFrame);
			addictiveFrame=-1;
		}
		for (int i=0;i< interp.animms [type].anims [numb].rsats.Length;i++)
		{
			if ((interp.animms [type].anims [numb].rsats[i].time<=frame)&&
			    (!interp.animms [type].anims [numb].rsats[i].played))
			{
				sManager.RandomizeSfx(efxSource, interp.animms [type].anims [numb].rsats[i].audios);
				interp.animms [type].anims [numb].rsats[i].played=true;
			}
			else if ((interp.animms [type].anims [numb].rsats[i].time >frame)&&
			         (interp.animms [type].anims [numb].rsats[i].played))
			{
				interp.animms [type].anims [numb].rsats[i].played=false;					
			}
		}
		if (isItRight >= 0) {
			for (int i=0; i<interp.animms[type].anims[realNumb].taos.Length; i++)
				if ((frame >= interp.animms [type].anims [realNumb].taos [i].time) && (interp.animms [type].anims [realNumb].taos [i].order != mov.settings.spriteLayerValue))
					sp.ChangeRenderOrder (interp.animms [type].anims [realNumb].taos [i].order, mov.gameObject);
		}
		else 
		{
			for (int i=0; i<interp.animms[type].anims[realNumb].ltaos.Length; i++)
				if ((frame >= interp.animms [type].anims [realNumb].ltaos [i].time) && (interp.animms [type].anims [realNumb].ltaos [i].order != mov.settings.spriteLayerValue))
					sp.ChangeRenderOrder (interp.animms [type].anims [realNumb].ltaos [i].order, mov.gameObject);
		}
	}
}
