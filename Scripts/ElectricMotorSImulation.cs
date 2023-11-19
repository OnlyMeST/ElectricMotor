using UnityEngine;
using System.IO;

public class ElectricMotorSimulation : MonoBehaviour
{
    // Simulation parameters
    public float voltage;
    public float current;
    public float resistance;
    public float torque;
    public float speed;
    public float temperature;
    public float vibration;
    public float load;
    public float runTime;

    // Data logging
    private StreamWriter csvWriter;
    private float timeElapsed;

    // Parameters for malfunctions
    public float criticalTemperature1 = 80; // Motor starts malfunctioning when temperature reaches 80°C
    public float criticalTemperature2 = 100; // Motor shuts down when temperature reaches 100°C
    public float spikeStartTime = 5; // Time when voltage spikes start (seconds)
    public float spikeEndTime = 10; // Time when voltage spikes end (seconds)
    public float spikeAmplitude = 50; // Voltage amplitude during spikes
    public float bearingWearFactor = 0.5f; // Vibration increases due to bearing wear
    public float loadImbalanceStartTime = 15; // Time when load imbalance starts (seconds)
    public float loadImbalanceEndTime = 20; // Time when load imbalance ends (seconds)
    public float loadImbalanceFactor = 1.2f; // Load imbalance factor (increased load)
    public float insulationBreakdownTime = 30; // Time when insulation breakdown occurs (seconds)
    public float insulationBreakdownDuration = 5; // Duration of the insulation breakdown effect (seconds)
    public float insulationBreakdownMultiplier = 2.5f; // Multiplier for voltage and current during the breakdown


    void Start()
    {
        // Data logging
        string filePath = Application.dataPath + "/ElectricMotorData.csv";
        csvWriter = new StreamWriter(filePath);
        csvWriter.WriteLine("Time, Voltage, Current, Resistance, Torque, Speed, Temperature, Vibration, Load");
    }

    void Update()
    {
        timeElapsed += Time.deltaTime; //Simulation Time
        voltage = Mathf.Sin(timeElapsed) * 100; // Varying voltage between -100V to 100V over time
        current = timeElapsed * 10; // Linearly increasing current at a rate of 10A per second
        resistance = 20; // Resistance remains constant at 20Ω
        torque = 100 - timeElapsed; // Torque decreases over time from 100N.m to 0N.m
        speed = timeElapsed * 100; // Speed increases over time at a rate of 100 RPM per second
        temperature = timeElapsed * 5; // Temperature increases linearly at a rate of 5°C per second
        vibration = Mathf.Abs(Mathf.Sin(timeElapsed)); // Vibration varies between 0 and 1 periodically
        load = 500; // Load remains constant at 500N

        // Simulate overheating
        if (temperature >= criticalTemperature1)
        {
            torque = 100 - (timeElapsed * 1.1f);
            current = timeElapsed * 10.25f;
        }
        if (temperature >= criticalTemperature2)
        {
            Destroy(gameObject);
        }
        // Simulate voltage spikes or drops
        if (timeElapsed > spikeStartTime && timeElapsed < spikeEndTime)
        {
            voltage += spikeAmplitude;
        }
        // Simulate bearing wear
        vibration += Mathf.Sin(timeElapsed) * bearingWearFactor;
        // Simulate load imbalance
        if (timeElapsed > loadImbalanceStartTime && timeElapsed < loadImbalanceEndTime)
        {
            load = load * loadImbalanceFactor;
        }
        if (timeElapsed > insulationBreakdownTime && timeElapsed < insulationBreakdownTime + insulationBreakdownDuration)
        {
            voltage *= insulationBreakdownMultiplier;
            current *= insulationBreakdownMultiplier;
        }

        csvWriter.WriteLine($"{timeElapsed}, {voltage}, {current}, {resistance}, {torque}, {speed}, {temperature}, {vibration}, {load}");

        // Check if the simulation has reached the desired run time
        if (timeElapsed >= runTime)
        {
            csvWriter.Close();
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        csvWriter.Close();
    }
}
