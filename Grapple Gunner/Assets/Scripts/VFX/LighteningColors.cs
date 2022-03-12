using UnityEngine;

[CreateAssetMenu(fileName = "Lightening Colors", menuName = "Grapple Gunner/VFX/LighteningColors", order = 0)]
public class LighteningColors : ScriptableObject
{
    public Color standardColor = Color.white;
    public Color redColor = Color.red;
    public Color orangeColor = new Color(255f / 255f, 165f / 255f, 0);
    public Color greenColor = Color.green;
    public Color blueColor = Color.blue;
}