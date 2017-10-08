using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

public struct Splat {
	public Matrix4x4 splatMatrix;
	public Vector4 channelMask;
	public Vector4 scaleBias;
}

[System.Serializable]
public class UpdateScoreEvent : UnityEvent<float> {
}

public class SplatManager : MonoBehaviour {

	public UpdateScoreEvent updateScore;

	public int sizeX;
	public int sizeY;

	public Texture2D splatTexture;
	public int splatsX = 4;
	public int splatsY = 4;

	public RenderTexture splatTex;
	public RenderTexture splatTexAlt;

  public RenderTexture worldPosTex;
	public RenderTexture worldPosTexTemp;
	public RenderTexture worldTangentTex;
	public RenderTexture worldBinormalTex;
	private Camera rtCamera;

	private Material splatBlitMaterial;

	private bool evenFrame = false;

	public Vector4 scores = Vector4.zero;

	public RenderTexture RTWorldUnmappedRed;  // Holds splatTex with unmapped UV coords filled in with red
	public RenderTexture RT256;
	public RenderTexture RT4;
	public Texture2D Tex4;

	private List<Splat> splatRenderQueue = new List<Splat>();

	// Add a splat to this SplatManager
	public void AddSplat(Matrix4x4 splatMatrix, Vector4 channelMask) {
		Splat newSplat;
		newSplat.splatMatrix = splatMatrix;
		newSplat.channelMask = channelMask;

		// Calculate a random offset in splatTex
		float splatscaleX = 1.0f / splatsX;
		float splatscaleY = 1.0f / splatsY;
		float splatsBiasX = Mathf.Floor( Random.Range(0,splatsX * 0.99f) ) / splatsX;
		float splatsBiasY = Mathf.Floor( Random.Range(0,splatsY * 0.99f) ) / splatsY;
		newSplat.scaleBias = new Vector4(splatscaleX, splatscaleY, splatsBiasX, splatsBiasY );

		splatRenderQueue.Add(newSplat);
	}

  // Use this for initialization
  void Start () {
		splatBlitMaterial = new Material (Shader.Find ("Splatoonity/SplatBlit"));
		
		splatTex = new RenderTexture (sizeX, sizeY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		splatTex.Create ();
		RTWorldUnmappedRed = new RenderTexture(sizeX, sizeY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		RTWorldUnmappedRed.Create();
		splatTexAlt = new RenderTexture (sizeX, sizeY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		splatTexAlt.Create ();
		worldPosTex = new RenderTexture (sizeX, sizeY, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
		worldPosTex.Create ();
		worldPosTexTemp = new RenderTexture (sizeX, sizeY, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
		worldPosTexTemp.Create ();
		worldTangentTex = new RenderTexture (sizeX, sizeY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		worldTangentTex.Create ();
		worldBinormalTex = new RenderTexture (sizeX, sizeY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		worldBinormalTex.Create();

		// Set the Splat texture on this object's renderer
		GetComponent<Renderer>().material.SetTexture("_SplatTex", splatTex);

		Shader.SetGlobalTexture ("_WorldPosTex", worldPosTex);
		Shader.SetGlobalTexture ("_WorldTangentTex", worldTangentTex);
		Shader.SetGlobalTexture ("_WorldBinormalTex", worldBinormalTex);
		Shader.SetGlobalVector ("_SplatTexSize", new Vector4 (sizeX, sizeY, 0, 0));

		// Textures for tallying scores
		RT256 = new RenderTexture (256, 256, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
		RT256.autoGenerateMips = true;
		RT256.useMipMap = true;
		RT256.Create ();
		RT4 = new RenderTexture (4, 4, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		RT4.Create ();
		Tex4 = new Texture2D (4, 4, TextureFormat.ARGB32, false);

		GameObject rtCameraObject = new GameObject ();
		rtCameraObject.name = "rtCameraObject";
		rtCameraObject.transform.position = Vector3.zero;
		rtCameraObject.transform.rotation = Quaternion.identity;
		rtCameraObject.transform.localScale = Vector3.one;
		rtCamera = rtCameraObject.AddComponent<Camera> ();
		rtCamera.clearFlags = CameraClearFlags.SolidColor;
		rtCamera.backgroundColor = new Color (0, 0, 0, 0);
		rtCamera.orthographic = true;
		rtCamera.nearClipPlane = 0.0f;
		rtCamera.farClipPlane = 1.0f;
		rtCamera.orthographicSize = 1.0f;
		rtCamera.aspect = 1.0f;
		rtCamera.useOcclusionCulling = false;
		rtCamera.enabled = false;

		RenderTextures();
		BleedTextures();
		StartCoroutine(UpdateScores());
  }

	// Render textures with a command buffer.
	// This is more flexible as you can explicitly add more objects to render without worying about layers.
	// You could also have multiple instances for chunks of a scene.
	void RenderTextures() {

		// Set the culling mask to Nothing so we can draw renderers explicitly
		rtCamera.cullingMask = LayerMask.NameToLayer("Nothing");

		Material worldPosMaterial = new Material (Shader.Find ("Splatoonity/WorldPosUnwrap"));
		Material worldTangentMaterial = new Material (Shader.Find ("Splatoonity/WorldTangentUnwrap"));
		Material worldBiNormalMaterial = new Material (Shader.Find ("Splatoonity/WorldBinormalUnwrap"));

		// You could collect all objects you want rendererd and loop through DrawRenderer
		// but for this example I'm just drawing the one renderer.
		Renderer envRenderer = this.gameObject.GetComponent<Renderer> ();

		// You could also use a multi render target and only have to draw each renderer once.
		CommandBuffer cb = new CommandBuffer();
		cb.SetRenderTarget(worldPosTex);
		cb.ClearRenderTarget(true, true, new Color(0,0,0,0));
		cb.DrawRenderer(envRenderer, worldPosMaterial);

		cb.SetRenderTarget(worldTangentTex);
		cb.ClearRenderTarget(true, true, new Color(0,0,0,0));
		cb.DrawRenderer(envRenderer, worldTangentMaterial);

		cb.SetRenderTarget(worldBinormalTex);
		cb.ClearRenderTarget(true, true, new Color(0,0,0,0));
		cb.DrawRenderer(envRenderer, worldBiNormalMaterial);

		// Only have to render the camera once!
		rtCamera.AddCommandBuffer(CameraEvent.AfterEverything, cb);
		rtCamera.Render();
	}


	void BleedTextures() {
		Graphics.Blit (Texture2D.blackTexture, splatTex, splatBlitMaterial, 1);		
		Graphics.Blit (Texture2D.blackTexture, splatTexAlt, splatBlitMaterial, 1);

		splatBlitMaterial.SetVector("_SplatTexSize", new Vector2( sizeX, sizeY ) );

		// Bleed the world position out 2 pixels
		Graphics.Blit (worldPosTex, worldPosTexTemp, splatBlitMaterial, 2);
		Graphics.Blit (worldPosTexTemp, worldPosTex, splatBlitMaterial, 2);

		// Don't need this guy any more
		worldPosTexTemp.Release();
		worldPosTexTemp = null;
	}


	// Blit the splats
	// This is similar to how a deferred decal would work
	// except instead of getting the world position from the depth
	// use the world position that is stored in the texture.
	// Each splat is tested against the entire world position texture.
	void PaintSplats() {

		if (splatRenderQueue.Count > 0) {
			
			Matrix4x4[] SplatMatrixArray = new Matrix4x4[10];
			Vector4[] SplatScaleBiasArray = new Vector4[10];
			Vector4[] SplatChannelMaskArray = new Vector4[10];

			// Render up to 10 splats per frame.
			int i = 0;
			while(splatRenderQueue.Count > 0 && i < 10 ){
				SplatMatrixArray [i] = splatRenderQueue[0].splatMatrix;
				SplatScaleBiasArray [i] = splatRenderQueue[0].scaleBias;
				SplatChannelMaskArray [i] = splatRenderQueue[0].channelMask;
				splatRenderQueue.RemoveAt(0);
				i++;
			}
			splatBlitMaterial.SetMatrixArray ( "_SplatMatrix", SplatMatrixArray );
			splatBlitMaterial.SetVectorArray ( "_SplatScaleBias", SplatScaleBiasArray );
			splatBlitMaterial.SetVectorArray ( "_SplatChannelMask", SplatChannelMaskArray );

			splatBlitMaterial.SetInt ( "_TotalSplats", i );

			splatBlitMaterial.SetTexture ("_WorldPosTex", worldPosTex);

			// Ping pong between the buffers to properly blend splats.
			// If this were a compute shader you could just update one buffer.
			if (evenFrame) {
				splatBlitMaterial.SetTexture ("_LastSplatTex", splatTexAlt);
				Graphics.Blit (splatTexture, splatTex, splatBlitMaterial, 0);
				Shader.SetGlobalTexture ("_SplatTex", splatTex);
				evenFrame = false;
			} else {
				splatBlitMaterial.SetTexture ("_LastSplatTex", splatTex);
				Graphics.Blit (splatTexture, splatTexAlt, splatBlitMaterial, 0);
				Shader.SetGlobalTexture ("_SplatTex", splatTexAlt);
				evenFrame = true;
			}

		}

	}
		
	// Update the scores by mipping the splat texture down to a 4x4 texture and sampling the pixels.
	// Space the whole operation out over a few frames to keep everything running smoothly.
	// Only update the scores once every second.
	IEnumerator UpdateScores() {

		while( true ){

			yield return new WaitForEndOfFrame();

			splatBlitMaterial.SetTexture ("_WorldPosTex", worldPosTex);
			// Shader pass 4 fills in RTWorldUnmappedRed with the splatTex + world pos tex if it is zero
			Graphics.Blit (splatTex, RTWorldUnmappedRed, splatBlitMaterial, 4);
			Graphics.Blit (RTWorldUnmappedRed, RT256, splatBlitMaterial, 3);
			Graphics.Blit (RT256, RT4);

			RenderTexture.active = RT4;
			Tex4.ReadPixels (new Rect (0, 0, 4, 4), 0, 0);
			Tex4.Apply ();

			yield return new WaitForSeconds(0.01f);

			Color scoresColor = new Color(0,0,0,0);
			scoresColor += Tex4.GetPixel(0,0);
			scoresColor += Tex4.GetPixel(0,1);
			scoresColor += Tex4.GetPixel(0,2);
			scoresColor += Tex4.GetPixel(0,3);

			yield return new WaitForSeconds(0.01f);

			scoresColor += Tex4.GetPixel(1,0);
			scoresColor += Tex4.GetPixel(1,1);
			scoresColor += Tex4.GetPixel(1,2);
			scoresColor += Tex4.GetPixel(1,3);

			yield return new WaitForSeconds(0.01f);

			scoresColor += Tex4.GetPixel(2,0);
			scoresColor += Tex4.GetPixel(2,1);
			scoresColor += Tex4.GetPixel(2,2);
			scoresColor += Tex4.GetPixel(2,3);

			yield return new WaitForSeconds(0.01f);

			scoresColor += Tex4.GetPixel(3,0);
			scoresColor += Tex4.GetPixel(3,1);
			scoresColor += Tex4.GetPixel(3,2);
			scoresColor += Tex4.GetPixel(3,3);

			// Max value for each score is 16, since there are 16 pixels each with a value of 0-1
			scores.x = scoresColor.r / 16.0f;
			scores.y = scoresColor.g / 16.0f;
			scores.z = scoresColor.b / 16.0f;
			scores.w = scoresColor.a / 16.0f;

			updateScore.Invoke(scores.x);

			yield return new WaitForSeconds (1.0f);
		}

	}
	
	// Update is called once per frame
	void Update () {

		PaintSplats ();

	}
	
}
