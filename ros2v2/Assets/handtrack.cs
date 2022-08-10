using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.UnityRoboticsDemo;

public class handtrack : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "pos_rot";
    public float publishMessageFrequency = 0.5f;
    private float timeElapsed;
    public GameObject fingerObject;
    private float angleL;
    private float angleR;
    Vector3[] tipsL = new Vector3[3];
    Vector3[] tipsR = new Vector3[3];


    List<GameObject> fingerObjectsL = new List<GameObject>();
    List<GameObject> fingerObjectsR = new List<GameObject>();

    MixedRealityPose pose;


    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject obj = Instantiate(fingerObject, this.transform);
            fingerObjectsL.Add(obj);
            GameObject obj2 = Instantiate(fingerObject, this.transform);
            fingerObjectsR.Add(obj2);
        }
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PosRotMsg>(topicName);

    }

    void Update()
    {
        //only render if hand is tracked
        timeElapsed += Time.deltaTime;

        if (timeElapsed > publishMessageFrequency)
        {
            for (int i = 0; i < 3; i++)
            {
                fingerObjectsL[i].GetComponent<Renderer>().enabled = false;
                fingerObjectsR[i].GetComponent<Renderer>().enabled = false;
            }

            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Left, out pose))
            {
                tipsL[0] = pose.Position;
                fingerObjectsL[0].GetComponent<Renderer>().enabled = true;
            }
            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left, out pose))
            {
                tipsL[1] = pose.Position;
                fingerObjectsL[1].GetComponent<Renderer>().enabled = true;
            }


            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexKnuckle, Handedness.Left, out pose))
            {
                tipsL[2] = pose.Position;
                fingerObjectsL[2].GetComponent<Renderer>().enabled = true;
            }
            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out pose))
            {
                tipsR[0] = pose.Position;
                fingerObjectsR[0].GetComponent<Renderer>().enabled = true;
            }

            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out pose))
            {
                tipsR[1] = pose.Position;
                fingerObjectsR[1].GetComponent<Renderer>().enabled = true;
            }

            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexKnuckle, Handedness.Right, out pose))
            {
                tipsR[2] = pose.Position;
                fingerObjectsR[2].GetComponent<Renderer>().enabled = true;
            }

            for (int i = 0; i < 3; i++)
            {
                fingerObjectsL[i].transform.position = tipsL[i];
                fingerObjectsR[i].transform.position = tipsR[i];
            }
            float AngleL = Vector3.Angle(fingerObjectsL[2].transform.position - fingerObjectsL[0].transform.position, fingerObjectsL[2].transform.position - fingerObjectsL[1].transform.position);
            float angleR = Vector3.Angle(fingerObjectsR[2].transform.position - fingerObjectsR[0].transform.position, fingerObjectsR[2].transform.position - fingerObjectsR[1].transform.position);
            PosRotMsg cubePos = new PosRotMsg(
                fingerObjectsL[2].transform.position.x,
                fingerObjectsL[2].transform.position.y,
                fingerObjectsL[2].transform.position.z,
                fingerObjectsL[2].transform.rotation.x,
                fingerObjectsL[2].transform.rotation.y,
                fingerObjectsL[2].transform.rotation.z,
                fingerObjectsL[2].transform.rotation.w,
                AngleL,
                fingerObjectsR[2].transform.position.x,
                fingerObjectsR[2].transform.position.y,
                fingerObjectsR[2].transform.position.z,
                fingerObjectsR[2].transform.rotation.x,
                fingerObjectsR[2].transform.rotation.y,
                fingerObjectsR[2].transform.rotation.z,
                fingerObjectsR[2].transform.rotation.w,
                angleR);

            ros.Publish(topicName, cubePos);
            timeElapsed = 0;
        }
    }
}