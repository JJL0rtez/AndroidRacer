using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
/*
 * Requirements: 
 * 1. Must be able to generate consistant levels that seem random the user.
 * 2. Must implement jumps
 * 3. Must implement special track sections
 * 4. Must add coin and powerup pickups to each level.
 * 5. References a track practice config to ditermine if atribute can be used.
 * 
 *
**/

[ExecuteInEditMode]
public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private SpriteShapeController _spriteShapeController;

    //[SerializeField, Range(3f, 100f)] private int _sectionLength = 50;
    [SerializeField, Range(0f, 1f)] private float _curveSmoothness = 0.5f;
    [SerializeField, Range(0, 10)] private int _SmoothingSteps = 5;
    //[SerializeField] private float _noiseStep = 0.5f;
    [SerializeField] private float _bottom = 1.0f;
    [SerializeField, Range(0f, 5f)] private float _MinimumPositionDifference = 0.5f; 

    private Vector3 _lastPos;

    private Seed seed = new Seed();

    public void Start () {
        PsudoRandomGenerator psudoRandomGenerator = new PsudoRandomGenerator();
        _spriteShapeController.spline.Clear();
        float position = 0.0f;
        float lastPosition = -1.0f;  // the -1 is used to track if the value has ever been used by the do while
        for(int i = 0; i < seed.courseLength; i++)
        {   

            // Get height of point from psudo random generator
            do
            {
                position = psudoRandomGenerator.GetNextPosition(seed);
                
            }
            while(Math.Abs(position - lastPosition) < _MinimumPositionDifference && lastPosition != -1.0f);
           // Debug.Log("position == " + position);

            // Smoothing Point (placed 50% beteween previous point and next point)
            for(int j = 0; j < _SmoothingSteps; j++)
            {
           // Debug.Log("i = " + i + " seed.courseLength = " + seed.courseLength);
                //System.Threading.Thread.Sleep(1000);
                // Get position differance and assign that to the sprite shape controller
                // Position Math -> Math.abs(position - lastPosition)
                float smoothedPosition = getSmoothedPoint(position,lastPosition,j);
                _lastPos = transform.position + new Vector3(i * seed._frequency, (smoothedPosition));

               // Debug.Log("Height = " + smoothedPosition);
                
                if(i < seed.courseLength)
                {
                
                    _spriteShapeController.spline.InsertPointAt(i, _lastPos);
                    _spriteShapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                    _spriteShapeController.spline.SetLeftTangent(i, Vector3.left * seed._frequency * _curveSmoothness);
                    _spriteShapeController.spline.SetRightTangent(i, Vector3.right * seed._frequency * _curveSmoothness);
                }
                // increment i only if j != to _SmoothingSteps -1, otherwise it will skip an index on "InsertPointAt" and throw an error
                if(j != _SmoothingSteps -1)
                {
                    i++; 
                }
            }
           // Debug.Log("last pos  = " + _lastPos);
            // Assign current position to last position so the midpoint can be set next itteration
            lastPosition = position;
           /*
            // Set non-smoothed point
            _lastPos = transform.position + new Vector3(i * seed._frequency, position);
            _spriteShapeController.spline.InsertPointAt(i, _lastPos);
            // Set Tangents
            if(i != 0 && i != seed.courseLength -1)
            {
                _spriteShapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                _spriteShapeController.spline.SetLeftTangent(i, Vector3.left * seed._frequency * _curveSmoothness);
                _spriteShapeController.spline.SetRightTangent(i, Vector3.right * seed._frequency * _curveSmoothness);
            }
            /*/
        }
        // Set Thickness of Level
        _spriteShapeController.spline.InsertPointAt(seed.courseLength, new Vector3(_lastPos.x, transform.position.y - _bottom));
        _spriteShapeController.spline.InsertPointAt(seed.courseLength + 1, new Vector3(transform.position.x, transform.position.y - _bottom));

        // Place Jumps
        // 1. Scan generated course for peaks, and log the indexes
        // 2. Drop indexes from the list if there was more then then # of jumps on the seed
        //      The dropped indexes need to be the same every time but seem randomly placed
        // 3. Modify the height of the points to create a jump at the peak
        //      The width of the jump should be set in the seed (odd numbers only)

        // Scan for possiable jump locations and log them
        List<int> jumpIndexes = new List<int>();


        for (int  i = 1; i < seed.courseLength - 5; i++) // NEED TO REPLACE THE -5 WITH A PROPPER COURSE END.
        {
            // If the current points height > the last points height and current points height  is > next points height
            if(_spriteShapeController.spline.GetPosition(i).y > _spriteShapeController.spline.GetPosition(i - 1).y
            && _spriteShapeController.spline.GetPosition(i).y > _spriteShapeController.spline.GetPosition(i + 1).y)
            {
                Debug.Log("Added index");
                // Add current index if it was detectedas a peak.
                jumpIndexes.Add(i);
            }
            Debug.Log(_spriteShapeController.spline.GetPosition(i -1).y + " -- " + _spriteShapeController.spline.GetPosition(i).y + " -- " + _spriteShapeController.spline.GetPosition(i + 1).y);
        }

        Debug.Log("jump count 117 " + jumpIndexes.Count);
        // Drop indexes psudorandomly if the # of jumIndexes > the requested amount in the seed
        // Break if the jumpIndexes reaches the desired count.
        if(seed.jumps < jumpIndexes.Count)
        {
            Debug.Log("jump count 123 " + jumpIndexes.Count);
            // To decide if to drop use the following tests
            //    Example: 15.53 or 6.45
            // 1. First Pass   --> If first  # is even drop it.
            // 2. Second Pass  --> If Second # is even drop it.
            // 3. Third pass   --> If Third  # is even and > 5 drop it.

            string passNumber = "";
            if (seed.jumps < jumpIndexes.Count){
                // First Pass
                for(int i = 0; i < jumpIndexes.Count - 1; i++)
                {
                    // Convert height to a string in order to uses elementAt to get specific # in height
                    passNumber = _spriteShapeController.spline.GetPosition(i).y.ToString();
                    // Check if number is even
                    if(passNumber.ElementAt(0) % 2 == 0){
                        // Remove jump if true
                        jumpIndexes.RemoveAt(i);
                        // If at the disired number of jumps break
                        if(seed.jumps == jumpIndexes.Count){
                            break;
                        }
                    }
                }
                
                // Second pass
                if (seed.jumps != jumpIndexes.Count)
                {
                    for(int i = 0; i < jumpIndexes.Count - 1; i++)
                    {
                        // Convert height to a string in order to uses elementAt to get specific # in height
                        passNumber = _spriteShapeController.spline.GetPosition(i).y.ToString();
                        // Check if number is even
                        if(passNumber.ElementAt(1) % 2 == 0){
                            // Remove jump if true
                            jumpIndexes.RemoveAt(i);
                            // If at the disired number of jumps break
                            if(seed.jumps == jumpIndexes.Count){
                                break;
                            }
                        }
                    }
                }
                //Third pass
                if (seed.jumps != jumpIndexes.Count)
                {
                    for(int i = 0; i < jumpIndexes.Count - 1; i++)
                    {
                        // Convert height to a string in order to uses elementAt to get specific # in height
                        passNumber = _spriteShapeController.spline.GetPosition(i).y.ToString();
                        // Check if number is even && is  5 or less
                        if(passNumber.ElementAt(2) % 2 == 0 && passNumber.ElementAt(2) <= 5){
                            // Remove jump if true
                            jumpIndexes.RemoveAt(i);
                            // If at the disired number of jumps break
                            if(seed.jumps == jumpIndexes.Count){
                                break;
                            }
                        }
                    }
                }


                // Now set the jumpindexs height to -50 + the height of the surronding points if specified by the 
                int initialIndex;
                foreach(int index in jumpIndexes){
                    // to get the initial index
                    //      Subtract 1 and divide the jumpwidth by two
                    initialIndex = index - ((seed.jumpWidth)/2);
                    
                    for(int i = 0; i < seed.jumpWidth; i++)
                    {
                        _spriteShapeController.spline.InsertPointAt(initialIndex + i, new Vector3(_spriteShapeController.spline.GetPosition(initialIndex + i).x, -150, _spriteShapeController.spline.GetPosition(initialIndex + i).z));
                        _spriteShapeController.spline.RemovePointAt(initialIndex + i - 1);
                    }
                }
            }
        }

    }

    float getSmoothedPoint(float position, float lastposition, int smoothingPosition)
    {
        // position           -> Current position generated by the psudo-generator
        // lastPosition       -> The previous position generated by the psudo generator
        // smoothingPostition -> Represents the current value to use in the smoothing 
        //                          AKA: if smoothingPostition == 2 then the return point will be the lower value of either position or lastposition + 2/_SmoothingSteps

        if(position > lastposition){
            // Return lower value + smoothingPostition/_SmoothingSteps * Difference in positions
            // Debug.Log("Smooth")
            return MathF.Abs(lastposition +  MathF.Abs((position - lastposition) * smoothingPosition/_SmoothingSteps));
        }
        // Else lastposition is higher or both are equal
        else {
            return  MathF.Abs(lastposition - MathF.Abs((lastposition - position) * smoothingPosition/_SmoothingSteps));
        }
    }
}

