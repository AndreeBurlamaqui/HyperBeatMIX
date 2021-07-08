using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(Volume))]
public class CVDFilter : MonoBehaviour {
	enum ColorType { Normal, Protanopia, Protanomaly, Deuteranopia, Deuteranomaly, Tritanopia, Tritanomaly, Achromatopsia, Achromatomaly }

	[SerializeField] ColorType visionType = ColorType.Normal;
	ColorType currentVisionType;
	VolumeProfile[] profiles;
	Volume postProcessVolume;

	void Start () {
		currentVisionType = visionType;
		SetupVolume();
		LoadProfiles();
		ChangeProfile();
	}

	void Update () {
		if (visionType != currentVisionType) {
			currentVisionType = visionType;
			ChangeProfile();
		}
	}

	void SetupVolume () {
		postProcessVolume = GetComponent<Volume>();
		postProcessVolume.isGlobal = true;
	}

	void LoadProfiles () {
		Object[] profileObjects = Resources.LoadAll("Colorblind", typeof(VolumeProfile));
		profiles = new VolumeProfile[profileObjects.Length];
		for (int i = 0; i < profileObjects.Length; i++) {
			profiles[i] = (VolumeProfile)profileObjects[i];
		}
	}

	void ChangeProfile () {
		postProcessVolume.profile = profiles[(int)currentVisionType];
	}
}
