using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PianoTouchScript : MonoBehaviour {
	// NEWEST SCRIPT VERSION 1.1
	
	//Welcome!
	//For More info Check the ReadMe File in the Multi-Touch Piano Asset Folder and this Blog Post http://madcalfapps.blogspot.com/2014/11/multi-touch-piano-unity-asset.html
	
	public int movedowntopbar=0; //Move top bar down (good for Ad banners)
	private float H; //Height
	private float W; //Width
	private float keys; //White Keys on Screen
	private int yOffset; //Top Offset
	public bool hidesustain = false; //hide/show sustain button
	public bool sustain = true; //Sustain Toggle on & off    //BETTER TO KEEP SUSTAIN ON BY DEFAULT (ESPECIALLY FOR ANDROID)
	//	public bool autoloop = false; //Loops Audio Toggle (requires seamless audio file)
	private int OctaveOffset; //Pitch offset 
	private int TransposeOffset; //Pitch offset Transpose entire keyboard
	
	
	//IMPORTANT!!!
	// Instrument - Attach new sounds to the script on the main camera to change instrument audio
	// InstOffset - If you Add a new note you will need to change Inst1Offset,Inst2Offset,Inst3Offset to offset their pitch so a middle C plays the pitch of a middle C.
	//              If the imported sound is an "A" make int multiples of 12 (-12,0,12,24,36). if "B" (two notes higher) make int multiples of 12+2 (-10,2,14,26,38) 
	// Set Instuments Volume (set lower than 1.0f to avoid audio clipping with chords) 
	
	public AudioClip Instrument1;
	public int Inst1Offset=-22;//Pitch offset Instrument 1: Default -22
	public float Vol1=.4f; //Instrument 1 Volume: Default .4f
	public AudioClip Instrument2;
	public int Inst2Offset=-12;//Pitch offset Instrument 2: Default -12
	public float Vol2=.5f; //Instrument 2 Volume: Default .5f
	public AudioClip Instrument3;
	public int Inst3Offset=-21;//Pitch offset Instrument 3: Default -21
	public float Vol3=.5f; //Instrument 3 Volume: Default .5f
	
	//Touches
	private int fingerID;
	Vector2 mouse;
	Vector2 fingerPos1;
	Vector2 fingerPos2;
	Vector2 fingerPos3;
	Vector2 fingerPos4;
	Vector2 fingerPos5;
	Vector2 fingerPos6;
	Vector2 fingerPos7;
	Vector2 fingerPos8;
	Vector2 fingerPos9;
	Vector2 fingerPos10;
	
	// GUI Sliders v-keys, h-position 
	private float hSliderValue = 0.0F;
	private float vSliderValue = 10.0F;
	
	private float CurrentVol; //Instrument Volume Applied
	
	private float NoteOffVol=0.00F; //Volume of previous note played
	
	//Instrument Selected for Button Images
	private bool inst1 = true;
	private bool inst2 = false;
	private bool inst3 = false;
	
	//Textures
	public Texture2D Header;
	public Texture2D WhiteKey;
	public Texture2D WhiteKeyDown;
	public Texture2D BlackKey;
	public Texture2D BlackKeyDown;
	public Texture2D SustainButton_1;
	public Texture2D SustainButton_2;
	public Texture2D Inst1Button_1;
	public Texture2D Inst1Button_2;
	public Texture2D Inst2Button_1;
	public Texture2D Inst2Button_2;
	public Texture2D Inst3Button_1;
	public Texture2D Inst3Button_2;
	public GUIStyle style;
	
	//Arrays for Sounds
	GameObject[] Sounds1 = new GameObject[88];
	GameObject[] Sounds2 = new GameObject[88];
	
	//Arrays for Keys
	Rect[] Rects = new Rect[52];
	Rect[] topRects = new Rect[88];
	Rect[] playRects = new Rect[88];
	bool[] playedSound = new bool[88];
	int[] values1 = new int[] { 0, 2, 3, 5, 6, 7, 9, 10, 12, 13, 14, 16, 17, 19,20,21, 23,24, 26,27,28 ,30,31 ,33,34,35 ,37,38 ,40,41,42 ,44,45 ,47,48,49} ;
	float[] values2 = new float[] { 0, 1.5f, 2,-10,4.5f, 5,-10,-10,8.5f, 9,-10,11.5f, 12,-10,-10,15.5f, 16,-10,18.5f, 19,-10,-10,22.5f, 23,-10,25.5f, 26,-10,-10,29.5f, 30,-10,32.5f, 33,-10,-10,36.5f, 37,-10,39.5f, 40,-10,-10,43.5f, 44,-10,46.5f, 47,-10,-10,50.5f, 51} ;
	int[] pitches1 = new int[] { -24, -22, -21, -19, -17, -16, -14, -12, -10, -9, -7, -5, -4, -2,0,2, 3,5, 7,8,10 ,12,14 ,15,17,19 ,20,22 ,24,26,27 ,29,31 ,32,34,36,38,39,41,43,44,46,48,50,51,53,55,56,58,60,62,63,-23, -20, -18, -15, -13, -11, -8, -6, -3, -1, 1, 4, 6,9,11,13,16 ,18,21,23 ,25,28 ,30,33,35 ,37,40 ,42,45,47,49,52,54,57,59,61,64} ;
	
	//Toggle Note Instance (helps prevent pops in audio)
	int[] note = new int[88];
	
	private Rect RectHead; //Top Header
	
	
	void Start () {
		
		for (int i = 0; i < 88; i++)
		{
			//Create the game object
			Sounds1[i] = new GameObject();  
			Sounds1[i].gameObject.AddComponent<AudioSource>();
			Sounds1[i].GetComponent<AudioSource>().clip = Instrument1;
			
			Sounds2[i] = new GameObject();  
			Sounds2[i].gameObject.AddComponent<AudioSource>();
			Sounds2[i].GetComponent<AudioSource>().clip = Instrument1;
			
			playedSound[i]=false;
			note[i]= 1;
			
		}
		
		
		//INCREASE SIZE OF THE HEADER
		yOffset = Screen.height/5;
		
		//Set Default Volume
		CurrentVol=Vol1;
		
		//Note offset (can be different for each instrument)
		OctaveOffset=Inst1Offset;
		
		//Transpose the key of entire piano
		TransposeOffset=0;
		
		//remove "-yOffset" to move key offset to the bottom
		H = Screen.height-yOffset;
		W = Screen.width;
		
		//Number of white keys on the screen at one time
		keys = vSliderValue;
		
		//Initial scroll position of keys
		hSliderValue=W*62/keys/2-W;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER                                         
		#else
		
		//Touches code
		int allTouches = Input.touchCount;
		if(allTouches > 0)
		{
			for (int i = 0; i < allTouches; i++)
			{
				
				//Track the touch
				Touch touch = Input.GetTouch(i);
				Vector2 fingerPos = Input.GetTouch(i).position;
				TouchPhase phase = touch.phase; 
				
				
				//Flip Touch Position
				fingerPos.y = Screen.height - fingerPos.y;
				
				switch(phase)
				{
				case TouchPhase.Began:
					for (int y = 0; y < 88; y++)
					{
						if (playRects[y].Contains(fingerPos)||topRects[y].Contains(fingerPos)){
							if(note[y]==1){
								if(Sounds1[y].GetComponent<AudioSource>().isPlaying){
									Sounds1[y].GetComponent<AudioSource>().Stop();
								}
								Sounds1[y].GetComponent<AudioSource>().pitch=Mathf.Pow (2f,(pitches1[y]+OctaveOffset+TransposeOffset)/12f);
								Sounds1[y].GetComponent<AudioSource>().volume=CurrentVol;
								Sounds2[y].GetComponent<AudioSource>().volume=NoteOffVol;
								//								if(autoloop){
								//									Sounds1[y].GetComponent<AudioSource>().loop=true;
								//									Sounds2[y].GetComponent<AudioSource>().loop=false;
								//								}
								Sounds1[y].GetComponent<AudioSource>().Play();
								
								playedSound[y] = true;
								note[y]=2;
							}else{
								if(Sounds2[y].GetComponent<AudioSource>().isPlaying){
									Sounds2[y].GetComponent<AudioSource>().Stop();
								}
								Sounds2[y].GetComponent<AudioSource>().pitch=Mathf.Pow (2f,(pitches1[y]+OctaveOffset+TransposeOffset)/12f);
								Sounds2[y].GetComponent<AudioSource>().volume=CurrentVol;
								Sounds1[y].GetComponent<AudioSource>().volume=NoteOffVol;
								//								if(autoloop){
								//									Sounds2[y].GetComponent<AudioSource>().loop=true;
								//									Sounds1[y].GetComponent<AudioSource>().loop=false;
								//								}
								Sounds2[y].GetComponent<AudioSource>().Play();
								
								playedSound[y] = true;
								note[y]=1;
							}
						}
					}
					break;
				case TouchPhase.Moved:
					for (int y = 0; y < 88; y++)
					{
						if (playRects[y].Contains(fingerPos)||topRects[y].Contains(fingerPos)){
							if(!playedSound[y])
							{
								if(note[y]==1){
									if(Sounds1[y].GetComponent<AudioSource>().isPlaying){
										Sounds1[y].GetComponent<AudioSource>().Stop();
									}
									Sounds1[y].GetComponent<AudioSource>().pitch=Mathf.Pow (2f,(pitches1[y]+OctaveOffset+TransposeOffset)/12f);
									Sounds1[y].GetComponent<AudioSource>().volume=CurrentVol;
									Sounds2[y].GetComponent<AudioSource>().volume=NoteOffVol;
									//									if(autoloop){
									//										Sounds1[y].GetComponent<AudioSource>().loop=true;
									//										Sounds2[y].GetComponent<AudioSource>().loop=false;
									//									}
									Sounds1[y].GetComponent<AudioSource>().Play();
									
									playedSound[y] = true;
									note[y]=2;
								}else{
									if(Sounds2[y].GetComponent<AudioSource>().isPlaying){
										Sounds2[y].GetComponent<AudioSource>().Stop();
									}
									Sounds2[y].GetComponent<AudioSource>().pitch=Mathf.Pow (2f,(pitches1[y]+OctaveOffset+TransposeOffset)/12f);
									Sounds2[y].GetComponent<AudioSource>().volume=CurrentVol;
									Sounds1[y].GetComponent<AudioSource>().volume=NoteOffVol;
									//									if(autoloop){
									//										Sounds1[y].GetComponent<AudioSource>().loop=true;
									//										Sounds2[y].GetComponent<AudioSource>().loop=false;
									//									}
									Sounds2[y].GetComponent<AudioSource>().Play();
									
									playedSound[y] = true;
									note[y]=1;
								}
							}
						}
					}
					break;
				case TouchPhase.Stationary:
					print("Touch index " + touch.fingerId + " is stationary at position " + touch.position);
					break;
				case TouchPhase.Ended:
					print("Touch index " + touch.fingerId + " ended at position " + touch.position);
					break;
				case TouchPhase.Canceled:
					print("Touch index " + touch.fingerId + " cancelled");
					break;
				}
				
				
				//Handle Touches Position
				if(allTouches>=1){
					if(allTouches==1){
						fingerPos1 = Input.GetTouch(0).position;
						fingerPos1.y = Screen.height - fingerPos1.y;
						fingerPos2 = Input.GetTouch(0).position;
						fingerPos2.y = Screen.height - fingerPos2.y;
						fingerPos3 = Input.GetTouch(0).position;
						fingerPos3.y = Screen.height - fingerPos3.y;
						fingerPos4 = Input.GetTouch(0).position;
						fingerPos4.y = Screen.height - fingerPos4.y;
						fingerPos5 = Input.GetTouch(0).position;
						fingerPos5.y = Screen.height - fingerPos5.y;
						fingerPos6 = Input.GetTouch(0).position;
						fingerPos6.y = Screen.height - fingerPos6.y;
						fingerPos7 = Input.GetTouch(0).position;
						fingerPos7.y = Screen.height - fingerPos7.y;
						fingerPos8 = Input.GetTouch(0).position;
						fingerPos8.y = Screen.height - fingerPos8.y;
						fingerPos9 = Input.GetTouch(0).position;
						fingerPos9.y = Screen.height - fingerPos9.y;
						fingerPos10 = Input.GetTouch(0).position;
						fingerPos10.y = Screen.height - fingerPos10.y;
					}else if(allTouches==2){
						fingerPos1 = Input.GetTouch(0).position;
						fingerPos1.y = Screen.height - fingerPos1.y;
						fingerPos2 = Input.GetTouch(1).position;
						fingerPos2.y = Screen.height - fingerPos2.y;
						fingerPos3 = Input.GetTouch(0).position;
						fingerPos3.y = Screen.height - fingerPos3.y;
						fingerPos4 = Input.GetTouch(0).position;
						fingerPos4.y = Screen.height - fingerPos4.y;
						fingerPos5 = Input.GetTouch(0).position;
						fingerPos5.y = Screen.height - fingerPos5.y;
						fingerPos6 = Input.GetTouch(0).position;
						fingerPos6.y = Screen.height - fingerPos6.y;
						fingerPos7 = Input.GetTouch(0).position;
						fingerPos7.y = Screen.height - fingerPos7.y;
						fingerPos8 = Input.GetTouch(0).position;
						fingerPos8.y = Screen.height - fingerPos8.y;
						fingerPos9 = Input.GetTouch(0).position;
						fingerPos9.y = Screen.height - fingerPos9.y;
						fingerPos10 = Input.GetTouch(0).position;
						fingerPos10.y = Screen.height - fingerPos10.y;
					}else if(allTouches==3){
						fingerPos1 = Input.GetTouch(0).position;
						fingerPos1.y = Screen.height - fingerPos1.y;
						fingerPos2 = Input.GetTouch(1).position;
						fingerPos2.y = Screen.height - fingerPos2.y;
						fingerPos3 = Input.GetTouch(2).position;
						fingerPos3.y = Screen.height - fingerPos3.y;
						fingerPos4 = Input.GetTouch(0).position;
						fingerPos4.y = Screen.height - fingerPos4.y;
						fingerPos5 = Input.GetTouch(0).position;
						fingerPos5.y = Screen.height - fingerPos5.y;
						fingerPos6 = Input.GetTouch(0).position;
						fingerPos6.y = Screen.height - fingerPos6.y;
						fingerPos7 = Input.GetTouch(0).position;
						fingerPos7.y = Screen.height - fingerPos7.y;
						fingerPos8 = Input.GetTouch(0).position;
						fingerPos8.y = Screen.height - fingerPos8.y;
						fingerPos9 = Input.GetTouch(0).position;
						fingerPos9.y = Screen.height - fingerPos9.y;
						fingerPos10 = Input.GetTouch(0).position;
						fingerPos10.y = Screen.height - fingerPos10.y;
					}else if(allTouches==4){
						fingerPos1 = Input.GetTouch(0).position;
						fingerPos1.y = Screen.height - fingerPos1.y;
						fingerPos2 = Input.GetTouch(1).position;
						fingerPos2.y = Screen.height - fingerPos2.y;
						fingerPos3 = Input.GetTouch(2).position;
						fingerPos3.y = Screen.height - fingerPos3.y;
						fingerPos4 = Input.GetTouch(3).position;
						fingerPos4.y = Screen.height - fingerPos4.y;
						fingerPos5 = Input.GetTouch(0).position;
						fingerPos5.y = Screen.height - fingerPos5.y;
						fingerPos6 = Input.GetTouch(0).position;
						fingerPos6.y = Screen.height - fingerPos6.y;
						fingerPos7 = Input.GetTouch(0).position;
						fingerPos7.y = Screen.height - fingerPos7.y;
						fingerPos8 = Input.GetTouch(0).position;
						fingerPos8.y = Screen.height - fingerPos8.y;
						fingerPos9 = Input.GetTouch(0).position;
						fingerPos9.y = Screen.height - fingerPos9.y;
						fingerPos10 = Input.GetTouch(0).position;
						fingerPos10.y = Screen.height - fingerPos10.y;
					}else if(allTouches==5){
						fingerPos1 = Input.GetTouch(0).position;
						fingerPos1.y = Screen.height - fingerPos1.y;
						fingerPos2 = Input.GetTouch(1).position;
						fingerPos2.y = Screen.height - fingerPos2.y;
						fingerPos3 = Input.GetTouch(2).position;
						fingerPos3.y = Screen.height - fingerPos3.y;
						fingerPos4 = Input.GetTouch(3).position;
						fingerPos4.y = Screen.height - fingerPos4.y;
						fingerPos5 = Input.GetTouch(4).position;
						fingerPos5.y = Screen.height - fingerPos5.y;
						fingerPos6 = Input.GetTouch(0).position;
						fingerPos6.y = Screen.height - fingerPos6.y;
						fingerPos7 = Input.GetTouch(0).position;
						fingerPos7.y = Screen.height - fingerPos7.y;
						fingerPos8 = Input.GetTouch(0).position;
						fingerPos8.y = Screen.height - fingerPos8.y;
						fingerPos9 = Input.GetTouch(0).position;
						fingerPos9.y = Screen.height - fingerPos9.y;
						fingerPos10 = Input.GetTouch(0).position;
						fingerPos10.y = Screen.height - fingerPos10.y;
					}else if(allTouches==6){
						fingerPos1 = Input.GetTouch(0).position;
						fingerPos1.y = Screen.height - fingerPos1.y;
						fingerPos2 = Input.GetTouch(1).position;
						fingerPos2.y = Screen.height - fingerPos2.y;
						fingerPos3 = Input.GetTouch(2).position;
						fingerPos3.y = Screen.height - fingerPos3.y;
						fingerPos4 = Input.GetTouch(3).position;
						fingerPos4.y = Screen.height - fingerPos4.y;
						fingerPos5 = Input.GetTouch(4).position;
						fingerPos5.y = Screen.height - fingerPos5.y;
						fingerPos6 = Input.GetTouch(5).position;
						fingerPos6.y = Screen.height - fingerPos6.y;
						fingerPos7 = Input.GetTouch(0).position;
						fingerPos7.y = Screen.height - fingerPos7.y;
						fingerPos8 = Input.GetTouch(0).position;
						fingerPos8.y = Screen.height - fingerPos8.y;
						fingerPos9 = Input.GetTouch(0).position;
						fingerPos9.y = Screen.height - fingerPos9.y;
						fingerPos10 = Input.GetTouch(0).position;
						fingerPos10.y = Screen.height - fingerPos10.y;
					}else if(allTouches==7){
						fingerPos1 = Input.GetTouch(0).position;
						fingerPos1.y = Screen.height - fingerPos1.y;
						fingerPos2 = Input.GetTouch(1).position;
						fingerPos2.y = Screen.height - fingerPos2.y;
						fingerPos3 = Input.GetTouch(2).position;
						fingerPos3.y = Screen.height - fingerPos3.y;
						fingerPos4 = Input.GetTouch(3).position;
						fingerPos4.y = Screen.height - fingerPos4.y;
						fingerPos5 = Input.GetTouch(4).position;
						fingerPos5.y = Screen.height - fingerPos5.y;
						fingerPos6 = Input.GetTouch(5).position;
						fingerPos6.y = Screen.height - fingerPos6.y;
						fingerPos7 = Input.GetTouch(6).position;
						fingerPos7.y = Screen.height - fingerPos7.y;
						fingerPos8 = Input.GetTouch(0).position;
						fingerPos8.y = Screen.height - fingerPos8.y;
						fingerPos9 = Input.GetTouch(0).position;
						fingerPos9.y = Screen.height - fingerPos9.y;
						fingerPos10 = Input.GetTouch(0).position;
						fingerPos10.y = Screen.height - fingerPos10.y;
					}else if(allTouches==8){
						fingerPos1 = Input.GetTouch(0).position;
						fingerPos1.y = Screen.height - fingerPos1.y;
						fingerPos2 = Input.GetTouch(1).position;
						fingerPos2.y = Screen.height - fingerPos2.y;
						fingerPos3 = Input.GetTouch(2).position;
						fingerPos3.y = Screen.height - fingerPos3.y;
						fingerPos4 = Input.GetTouch(3).position;
						fingerPos4.y = Screen.height - fingerPos4.y;
						fingerPos5 = Input.GetTouch(4).position;
						fingerPos5.y = Screen.height - fingerPos5.y;
						fingerPos6 = Input.GetTouch(5).position;
						fingerPos6.y = Screen.height - fingerPos6.y;
						fingerPos7 = Input.GetTouch(6).position;
						fingerPos7.y = Screen.height - fingerPos7.y;
						fingerPos8 = Input.GetTouch(7).position;
						fingerPos8.y = Screen.height - fingerPos8.y;
						fingerPos9 = Input.GetTouch(0).position;
						fingerPos9.y = Screen.height - fingerPos9.y;
						fingerPos10 = Input.GetTouch(0).position;
						fingerPos10.y = Screen.height - fingerPos10.y;
					}else if(allTouches==9){
						fingerPos1 = Input.GetTouch(0).position;
						fingerPos1.y = Screen.height - fingerPos1.y;
						fingerPos2 = Input.GetTouch(1).position;
						fingerPos2.y = Screen.height - fingerPos2.y;
						fingerPos3 = Input.GetTouch(2).position;
						fingerPos3.y = Screen.height - fingerPos3.y;
						fingerPos4 = Input.GetTouch(3).position;
						fingerPos4.y = Screen.height - fingerPos4.y;
						fingerPos5 = Input.GetTouch(4).position;
						fingerPos5.y = Screen.height - fingerPos5.y;
						fingerPos6 = Input.GetTouch(5).position;
						fingerPos6.y = Screen.height - fingerPos6.y;
						fingerPos7 = Input.GetTouch(6).position;
						fingerPos7.y = Screen.height - fingerPos7.y;
						fingerPos8 = Input.GetTouch(7).position;
						fingerPos8.y = Screen.height - fingerPos8.y;
						fingerPos9 = Input.GetTouch(8).position;
						fingerPos9.y = Screen.height - fingerPos9.y;
						fingerPos10 = Input.GetTouch(0).position;
						fingerPos10.y = Screen.height - fingerPos10.y;
					}else if(allTouches>=10){
						fingerPos1 = Input.GetTouch(0).position;
						fingerPos1.y = Screen.height - fingerPos1.y;
						fingerPos2 = Input.GetTouch(1).position;
						fingerPos2.y = Screen.height - fingerPos2.y;
						fingerPos3 = Input.GetTouch(2).position;
						fingerPos3.y = Screen.height - fingerPos3.y;
						fingerPos4 = Input.GetTouch(3).position;
						fingerPos4.y = Screen.height - fingerPos4.y;
						fingerPos5 = Input.GetTouch(4).position;
						fingerPos5.y = Screen.height - fingerPos5.y;
						fingerPos6 = Input.GetTouch(5).position;
						fingerPos6.y = Screen.height - fingerPos6.y;
						fingerPos7 = Input.GetTouch(6).position;
						fingerPos7.y = Screen.height - fingerPos7.y;
						fingerPos8 = Input.GetTouch(7).position;
						fingerPos8.y = Screen.height - fingerPos8.y;
						fingerPos9 = Input.GetTouch(8).position;
						fingerPos9.y = Screen.height - fingerPos9.y;
						fingerPos10 = Input.GetTouch(9).position;
						fingerPos10.y = Screen.height - fingerPos10.y;
					}
					
					
					
					
					for (int y = 0; y < 88; y++)
					{
						if (!playRects[y].Contains(fingerPos1)&&!topRects[y].Contains(fingerPos1)&&!playRects[y].Contains(fingerPos2)&&!topRects[y].Contains(fingerPos2)&&!playRects[y].Contains(fingerPos3)&&!topRects[y].Contains(fingerPos3)&&!playRects[y].Contains(fingerPos4)&&!topRects[y].Contains(fingerPos4)&&!playRects[y].Contains(fingerPos5)&&!topRects[y].Contains(fingerPos5)&&!playRects[y].Contains(fingerPos6)&&!topRects[y].Contains(fingerPos6)&&!playRects[y].Contains(fingerPos7)&&!topRects[y].Contains(fingerPos7)&&!playRects[y].Contains(fingerPos8)&&!topRects[y].Contains(fingerPos8)&&!playRects[y].Contains(fingerPos9)&&!topRects[y].Contains(fingerPos9)&&!playRects[y].Contains(fingerPos10)&&!topRects[y].Contains(fingerPos10)){
							playedSound[y] = false;
							if(sustain==false){
								if(Sounds1[y].GetComponent<AudioSource>().isPlaying){
									Sounds1[y].GetComponent<AudioSource>().volume=NoteOffVol;
									//									Sounds1[y].GetComponent<AudioSource>().Stop();
									//									if(autoloop){
									//										Sounds1[y].GetComponent<AudioSource>().loop=true;
									//									}
								}
								if(Sounds2[y].GetComponent<AudioSource>().isPlaying){
									Sounds2[y].GetComponent<AudioSource>().volume=NoteOffVol;
									//									Sounds2[y].GetComponent<AudioSource>().Stop();
									//									if(autoloop){
									//										Sounds2[y].GetComponent<AudioSource>().loop=true;
									//									}
								}
							}
						}
					}
					
					
					
					
					
				}
				
			}
			
			
			
		}else{
			
			for (int i = 0; i < 88; i++)
			{
				playedSound[i] = false;
			}
			
			if(sustain==false){
				
				for (int i = 0; i < 88; i++)
				{
					Sounds1[i].GetComponent<AudioSource>().volume=NoteOffVol;
					Sounds2[i].GetComponent<AudioSource>().volume=NoteOffVol;
					
					//					if(Sounds1[i].GetComponent<AudioSource>().isPlaying){
					//						Sounds1[i].GetComponent<AudioSource>().Stop();
					//					}
					//					if(Sounds2[i].GetComponent<AudioSource>().isPlaying){
					//						Sounds2[i].GetComponent<AudioSource>().Stop();
					//					}
					
					
					//					if(autoloop){
					//						Sounds1[i].GetComponent<AudioSource>().loop=false;
					//						Sounds2[i].GetComponent<AudioSource>().loop=false;
					//					}
				}
				
			}
			
			
			
		}
		#endif
	}
	
	void MouseUpMethod() {
		//Mouse Up Method
		//settings for non-touch device
		for (int i = 0; i < 88; i++)
		{
			playedSound[i] = false;
		}
		
		if(sustain==false){
			
			for (int i = 0; i < 88; i++)
			{
				Sounds1[i].GetComponent<AudioSource>().volume=NoteOffVol;
				Sounds2[i].GetComponent<AudioSource>().volume=NoteOffVol;
			}
			
		}
	}
	
	void OnGUI() {
		
		//settings for non-touch device
		#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER                                         
		
		if (Event.current.type == EventType.MouseDown||Event.current.type == EventType.MouseDrag){
			
			Vector2 fingerPos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
			
			for (int i = 0; i < 88; i++)
			{
				if (playRects[i].Contains(fingerPos)||topRects[i].Contains(fingerPos)){
					if(!playedSound[i])
					{
						if(note[i]==1){
							if(Sounds1[i].GetComponent<AudioSource>().isPlaying){
								Sounds1[i].GetComponent<AudioSource>().Stop();
							}
							Sounds1[i].GetComponent<AudioSource>().pitch=Mathf.Pow (2f,(pitches1[i]+OctaveOffset+TransposeOffset)/12f);
							Sounds1[i].GetComponent<AudioSource>().volume=CurrentVol;
							Sounds2[i].GetComponent<AudioSource>().volume=NoteOffVol;
							Sounds1[i].GetComponent<AudioSource>().Play();
							playedSound[i] = true;
							note[i]=2;
						}else{
							if(Sounds2[i].GetComponent<AudioSource>().isPlaying){
								Sounds2[i].GetComponent<AudioSource>().Stop();
							}
							Sounds2[i].GetComponent<AudioSource>().pitch=Mathf.Pow (2f,(pitches1[i]+OctaveOffset+TransposeOffset)/12f);
							Sounds2[i].GetComponent<AudioSource>().volume=CurrentVol;
							Sounds1[i].GetComponent<AudioSource>().volume=NoteOffVol;
							Sounds2[i].GetComponent<AudioSource>().Play();
							playedSound[i] = true;
							note[i]=1;
						}
					}
				}
			}
			
			for (int i = 0; i < 88; i++)
			{
				if (!playRects[i].Contains(fingerPos)&&!topRects[i].Contains(fingerPos)){
					playedSound[i] = false;
					if(sustain==false){
						if(Sounds1[i].GetComponent<AudioSource>().isPlaying){
							Sounds1[i].GetComponent<AudioSource>().volume=NoteOffVol;
						}
						if(Sounds2[i].GetComponent<AudioSource>().isPlaying){
							Sounds2[i].GetComponent<AudioSource>().volume=NoteOffVol;
						}
					}
				}
			}
			
			
			
		}else if (Event.current.type == EventType.MouseUp){
			
			MouseUpMethod();			
			
		}
		#endif
		
		
		//Top Bar Area for Controls
		RectHead = new Rect(0,0+movedowntopbar,W,yOffset);
		GUI.DrawTexture(RectHead, Header);
		
		//Sliders for Position and Range
		GUI.skin.horizontalSliderThumb = style;
		hSliderValue = GUI.HorizontalSlider(new Rect(yOffset/4, yOffset/2-yOffset/10+movedowntopbar, W/5, 40), hSliderValue, 0.0F, W*52/keys-W);
		vSliderValue = GUI.HorizontalSlider(new Rect(W-yOffset/4-W/5, yOffset/2-yOffset/10+movedowntopbar, W/5, 40), vSliderValue, 5.0F, 20f);
		keys = vSliderValue;
		
		
		Rect position2 = new Rect(W/2-yOffset/4*2-yOffset/8, yOffset/2-yOffset/4+movedowntopbar, yOffset/2, yOffset/2);
		Rect position3 = new Rect(W/2-yOffset/4*-1-yOffset/8, yOffset/2-yOffset/4+movedowntopbar, yOffset/2, yOffset/2);
		Rect position4 = new Rect(W/2-yOffset/4*-4-yOffset/8, yOffset/2-yOffset/4+movedowntopbar, yOffset/2, yOffset/2);
		
		
		if(hidesustain == true){
			//no sustain
		}else{
			Rect position1 = new Rect(W/2-yOffset/4*5-yOffset/8, yOffset/2-yOffset/4+movedowntopbar, yOffset/2, yOffset/2);
			
			if(sustain == true){
				GUI.DrawTexture(position1 , SustainButton_1);
			}else{
				GUI.DrawTexture(position1 , SustainButton_2);
			}
			if (GUI.Button(position1 , "", new GUIStyle())){
				if(sustain == true){
					sustain = false;
				}else{
					sustain = true;
				}
			}
			
		}
		
		if(inst1 == true){
			GUI.DrawTexture(position2 , Inst1Button_1);
			GUI.DrawTexture(position3 , Inst2Button_2);
			GUI.DrawTexture(position4 , Inst3Button_2);
		}else if(inst2 == true){
			GUI.DrawTexture(position2 , Inst1Button_2);
			GUI.DrawTexture(position3 , Inst2Button_1);
			GUI.DrawTexture(position4 , Inst3Button_2);
		}else if(inst3 == true){
			GUI.DrawTexture(position2 , Inst1Button_2);
			GUI.DrawTexture(position3 , Inst2Button_2);
			GUI.DrawTexture(position4 , Inst3Button_1);
		}
		
		
		
		
		//Change Instument Sounds
		if (GUI.Button(position2 , "", new GUIStyle())){
			inst1 = true;
			inst2 = false;
			inst3 = false;
			OctaveOffset=Inst1Offset;
			
			//Set New Audio
			for (int i = 0; i < 88; i++)
			{
				Sounds1[i].GetComponent<AudioSource>().clip = Instrument1;
				Sounds2[i].GetComponent<AudioSource>().clip = Instrument1;
			}
			
			//Set Volume
			CurrentVol=Vol1;
			
		}
		
		if (GUI.Button(position3 , "", new GUIStyle())){
			inst1 = false;
			inst2 = true;
			inst3 = false;
			OctaveOffset=Inst2Offset;
			
			//Set New Audio
			for (int i = 0; i < 88; i++)
			{
				Sounds1[i].GetComponent<AudioSource>().clip = Instrument2;
				Sounds2[i].GetComponent<AudioSource>().clip = Instrument2;
			}
			
			//Set Volume
			CurrentVol=Vol2;
			
		}
		
		if (GUI.Button(position4 , "", new GUIStyle())){
			inst1 = false;
			inst2 = false;
			inst3 = true;
			OctaveOffset=Inst3Offset;
			
			//Set New Audio
			for (int i = 0; i < 88; i++)
			{
				Sounds1[i].GetComponent<AudioSource>().clip = Instrument3;
				Sounds2[i].GetComponent<AudioSource>().clip = Instrument3;
			}
			
			//Set Volume
			CurrentVol=Vol3;
		}
		
		//Key Textures
		for (int i = 0; i < 88; i++)
		{
			if(i<52){
				
				playRects[i] = new Rect(W/keys*i-hSliderValue,yOffset+H/2+movedowntopbar/2,W/keys,H/2-movedowntopbar/2);
				Rects[i] = new Rect(W/keys*i-hSliderValue,yOffset+movedowntopbar,W/keys,H-movedowntopbar);
				
				if(playedSound[i]==false){
					GUI.DrawTexture(Rects[i], WhiteKey);
				}else{
					GUI.DrawTexture(Rects[i], WhiteKeyDown);
				}
				
				if(i==51){
					topRects[i] = new Rect(W/keys*values2[i]-hSliderValue,yOffset+movedowntopbar,W/keys,H/2-movedowntopbar/2);
				}else{
					topRects[i] = new Rect(W/keys*values2[i]-hSliderValue,yOffset+movedowntopbar,W/keys/2,H/2-movedowntopbar/2);
				}
				
			}else{
				
				playRects[i] = new Rect(W/keys/2+W/keys*values1[i-52]-hSliderValue,yOffset+movedowntopbar,W/keys,H/2-movedowntopbar/2);
				if(playedSound[i]==false){
					GUI.DrawTexture(playRects[i], BlackKey);
				}else{
					GUI.DrawTexture(playRects[i], BlackKeyDown);
				}
				
				
			}
			
			
			
		}
	}
}
