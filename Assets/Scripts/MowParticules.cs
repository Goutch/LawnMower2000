using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Random = System.Random;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

public class MowParticules : MonoBehaviour
{
    [SerializeField] private int maxParticules = 1000;
    [SerializeField] float particuleLifeTime = 2;
    [SerializeField] private float spawnRatePerSeconds = 0.1f;
    [SerializeField] private float initialSpeed = 0.7f;
    [SerializeField] private float initialUpVelocity = 0.7f;
    [SerializeField] private float gravity = -.98f;
    [SerializeField] private float lossOfMomentum = 1.4f;
    [SerializeField] private float minZ = 0.6f;
    [SerializeField] private float spread = 3;
    private Material material;
    private LawnMower lawnMower;
    private Mesh quad;
    private Matrix4x4[] particules;
    private Vector4[] colors;
    private Vector4[] velocities;
    private float[] timeToLive;
    private GameManager gameManager;
    MaterialPropertyBlock colorsProperty;
    private Random rnd;
    private int currentParticulesCount = 0;

    private void OnEnable()
    {
        rnd = new Random();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        lawnMower = GetComponent<LawnMower>();

        material = new Material(Shader.Find("Custom/ParticulesShader"));
        material.enableInstancing = true;

        quad = GeometryUtils.CreateQuad(1.0f / gameManager.Map.PixelsPerUnits, 1.0f / gameManager.Map.PixelsPerUnits);
        gameManager.OnGameStart += OnGameStart;
        particules = new Matrix4x4[maxParticules];
        velocities = new Vector4[maxParticules];
        timeToLive = new float[maxParticules];
        colors = new Vector4[maxParticules];
        colorsProperty = new MaterialPropertyBlock();
    }

    private void OnGameStart()
    {
        StartCoroutine(SpawnRoutine());
    }

    public void OnDestroy()
    {
        gameManager.OnGameStart -= OnGameStart;
    }

    private void Update()
    {
        for (int i = 0; i < currentParticulesCount; i++)
        {
            timeToLive[i] -= Time.deltaTime;
            if (timeToLive[i] <= 0)
            {
                currentParticulesCount--;
                particules[i] = particules[currentParticulesCount];
                timeToLive[i] = timeToLive[currentParticulesCount];
                velocities[i] = velocities[currentParticulesCount];
                colors[i] = colors[currentParticulesCount];
            }

            if (particules[i].m23 < minZ)
            {
                velocities[i].x *=1-(lossOfMomentum*Time.deltaTime);
                velocities[i].y *=1-(lossOfMomentum*Time.deltaTime);
                velocities[i].z -= (gravity * Time.deltaTime);
                Vector4 newPosition = new Vector4(
                    particules[i].m03 + (velocities[i].x * Time.deltaTime),
                    particules[i].m13 + (velocities[i].y * Time.deltaTime),
                    particules[i].m23 + (velocities[i].z * Time.deltaTime),
                    particules[i].m33);
                particules[i].SetColumn(3, newPosition);
            }
        }

        if (lawnMower.Mowing && currentParticulesCount != 0)
        {
            colorsProperty = new MaterialPropertyBlock();
            colorsProperty.SetVectorArray("_Colors", colors);
            Graphics.DrawMeshInstanced(quad, 0, material, particules, currentParticulesCount, colorsProperty);
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (lawnMower.Mowing)
        {
            yield return new WaitForSeconds(spawnRatePerSeconds);
            if (currentParticulesCount < maxParticules)
            {
                particules[currentParticulesCount] = Matrix4x4.Translate(new Vector3(transform.position.x, transform.position.y, .5f));
                velocities[currentParticulesCount] = new Vector4(
                    ((rnd.Next(-50, 50) / 100f) * spread) + (-transform.up.x * initialSpeed),
                    ((rnd.Next(-50, 50) / 100f) * spread) + (-transform.up.y * initialSpeed),
                    -initialUpVelocity,
                    0);
                timeToLive[currentParticulesCount] = particuleLifeTime;
                Color color;
                if (lawnMower.IsStuck)
                {
                    color = gameManager.Map.SampleMap(lawnMower.transform.position);
                }
                else
                {
                    Vector3 front = lawnMower.transform.position + lawnMower.transform.up * 0.5f;

                    color = gameManager.Map.SampleMap(
                        new Vector3(front.x + rnd.Next(-50, 50) / 200f,
                            front.y + rnd.Next(-50, 50) / 200f,
                            front.z));
                }

                colors[currentParticulesCount] = color;
                currentParticulesCount++;
            }

        }
    }
}