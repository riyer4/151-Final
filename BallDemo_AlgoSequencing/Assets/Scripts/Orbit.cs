using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlanetData  // Renamed from Orbit
{
    public string planetName = "Planet";
    public Transform planetTransform;
    public float orbitSpeed = 20f;
    public float orbitRadius = 5f;
    public float rotationSpeed = 30f;
    public Vector3 rotationAxis = Vector3.up;
    public bool startAtRandomAngle = false;

    [HideInInspector]
    public float currentOrbitAngle = 0f;
}

public class Orbit : MonoBehaviour
{
    [Header("Solar System Settings")]
    public List<PlanetData> planets = new List<PlanetData>();

    [Header("Optional")]
    public bool autoFindPlanets = false;

    void Start()
    {
        if (autoFindPlanets)
        {
            FindAllPlanets();
        }

        foreach (var planet in planets)
        {
            if (planet.planetTransform == null) continue;

            if (planet.startAtRandomAngle)
            {
                planet.currentOrbitAngle = Random.Range(0f, 360f);
            }

            UpdatePlanetPosition(planet);
        }
    }

    void Update()
    {
        foreach (var planet in planets)
        {
            if (planet.planetTransform == null) continue;

            planet.currentOrbitAngle += planet.orbitSpeed * Time.deltaTime;
            UpdatePlanetPosition(planet);
            planet.planetTransform.Rotate(planet.rotationAxis, planet.rotationSpeed * Time.deltaTime);
        }
    }

    void UpdatePlanetPosition(PlanetData planet)
    {
        float angleRad = planet.currentOrbitAngle * Mathf.Deg2Rad;
        float x = Mathf.Cos(angleRad) * planet.orbitRadius;
        float z = Mathf.Sin(angleRad) * planet.orbitRadius;
        planet.planetTransform.position = transform.position + new Vector3(x, 0, z);
    }

    void FindAllPlanets()
    {
        planets.Clear();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Planet"))
            {
                AddPlanet(child);
            }
        }
    }

    public void AddPlanet(Transform planetTransform)
    {
        PlanetData newPlanet = new PlanetData();
        newPlanet.planetTransform = planetTransform;
        newPlanet.planetName = planetTransform.name;
        planets.Add(newPlanet);
    }

    public PlanetData GetPlanet(string name)
    {
        return planets.Find(p => p.planetName == name);
    }

    public void SetOrbitSpeed(string planetName, float speed)
    {
        PlanetData planet = GetPlanet(planetName);
        if (planet != null) planet.orbitSpeed = speed;
    }
}
