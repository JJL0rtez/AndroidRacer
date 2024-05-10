using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed
{
    // Checks where to start grabing values from in the constant
    // Values are then used to apply variation to course to affect height
    [SerializeField] public int _psudoRandomInteger = 0;
    // Value to use to get offset from initial Integer
    [SerializeField] public int _psudoRandomIntegerOffset = 1;
    // Frequency of base Sin function
    [SerializeField] public float _frequency = 6.0f;
    // Amplitude of base Sin function
    [SerializeField] public float _amplitude = 0.35f;
    // # of Special Sections
    [SerializeField] public int _specialSections = 0;
    // # of Jumps
    [SerializeField] public int jumps  = 4;
    // Course length
    [SerializeField] public int courseLength = 100;
   
    // Only odd numbers
    [SerializeField] public int jumpWidth = 3;
}
