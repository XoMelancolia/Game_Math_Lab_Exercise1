using System.Collections;
using UnityEngine;

public class CraneSequenceController : MonoBehaviour
{
    public Transform crane;
    public Transform trolley;
    public Transform hook;
    public Transform concrete;
    public Transform cable;
    public Transform nearLimit;
    public Transform farLimit;
    public TrolleyController trolleyController;
    public float rotationSpeed = 20f;
    public float trolleySpeed = 10f;
    public float hookSpeed = 1f;
    public float waitTime = 1f;

    private bool isSequenceActive = false;

    void Update()
    {
        if (!isSequenceActive && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == concrete)
                {
                    StartCoroutine(ExecuteCraneSequence());
                }
            }
        }
    }

    IEnumerator ExecuteCraneSequence()
    {
        isSequenceActive = true;

        //rotate the crane
        yield return StartCoroutine(RotateCrane());

        //move the trolley
        yield return StartCoroutine(MoveTrolley());

        //lower the hook
        yield return StartCoroutine(LowerHook());

        //wait 1 sec
        yield return new WaitForSeconds(waitTime);

        //lift the concrete
        yield return StartCoroutine(LiftConcrete());

        
        //detach and move concrete
        DetachAndMoveConcrete();

        isSequenceActive = false;
    }

    IEnumerator RotateCrane()
    {
        Vector3 targetDirection = concrete.position - crane.position;
        targetDirection.y = 0; 
        Vector3 projectedTarget = Vector3.ProjectOnPlane(targetDirection, Vector3.up);
        Vector3 projectedForward = Vector3.ProjectOnPlane(-crane.right, Vector3.up);  //-crane.right since the arm faces negative x
        float angle = Vector3.SignedAngle(projectedForward, projectedTarget, Vector3.up);

        while (Mathf.Abs(angle) > 1.0f)
        {
            float step = rotationSpeed * Time.deltaTime;
            float rotationAmount = Mathf.Sign(angle) * step;
            crane.Rotate(0, rotationAmount, 0);
            angle -= rotationAmount;
            yield return null;
        }
    }

    IEnumerator MoveTrolley()
    {
        //current position of trolley along limits
        Vector3 currentTrolleyPositionXZ = new Vector3(trolley.position.x, 0, trolley.position.z);

        //concrete position projected onto XZ plane
        Vector3 concretePositionXZ = new Vector3(concrete.position.x, 0, concrete.position.z);

        //trolley near and far limits projected onto XZ plane
        Vector3 nearLimitPositionXZ = new Vector3(trolleyController.trolleyNearLimit.position.x, 0, trolleyController.trolleyNearLimit.position.z);
        Vector3 farLimitPositionXZ = new Vector3(trolleyController.trolleyFarLimit.position.x, 0, trolleyController.trolleyFarLimit.position.z);

        //total distance between the near and far limits
        float totalDistance = Vector3.Distance(nearLimitPositionXZ, farLimitPositionXZ);

        //current normalized position of the trolley between near and far limits
        float currentTrolleyNormalizedPosition = Vector3.Distance(nearLimitPositionXZ, currentTrolleyPositionXZ) / totalDistance;

        //target normalized position for trolley based on concrete position
        float targetTrolleyNormalizedPosition = Vector3.Distance(nearLimitPositionXZ, concretePositionXZ) / totalDistance;

        //move trolley towards target normalized position
        while (Mathf.Abs(currentTrolleyNormalizedPosition - targetTrolleyNormalizedPosition) > 0.01f)
        {
            //update current normalized position towards target
            currentTrolleyNormalizedPosition = Mathf.MoveTowards(currentTrolleyNormalizedPosition, targetTrolleyNormalizedPosition, trolleySpeed * Time.deltaTime / totalDistance);

            //update trolley position using normalized position
            trolleyController.MoveTrolley(currentTrolleyNormalizedPosition);

            yield return null;
        }

        //ensure trolley isaligned with concrete
        trolleyController.MoveTrolley(targetTrolleyNormalizedPosition);
    }

    IEnumerator LowerHook()
    {
        CableController cableController = cable.GetComponent<CableController>();

        SoftParenting concreteSoftParenting = concrete.GetComponent<SoftParenting>();

        while (!concreteSoftParenting.enabled && cable.localScale.y < cableController.maxCableLength)
        {
            //new cable length
            float currentLength = cable.localScale.y;
            float newLength = currentLength + hookSpeed * Time.deltaTime;

            //ensure maximum cable length isnt exceeded
            newLength = Mathf.Min(newLength, cableController.maxCableLength);

            //update the cable length
            cableController.ScaleCable((newLength - cableController.minCableLength) / (cableController.maxCableLength - cableController.minCableLength));

            yield return null;
        }
    }

    IEnumerator LiftConcrete()
    {
        CableController cableController = cable.GetComponent<CableController>();

        while (cable.localScale.y > cableController.minCableLength)
        {
            //calculate new cable length
            float currentLength = cable.localScale.y;
            float newLength = currentLength - hookSpeed * Time.deltaTime;

            //ensure cable length isnt less than minimum
            newLength = Mathf.Max(newLength, cableController.minCableLength);

            //update the cable length
            cableController.ScaleCable((newLength - cableController.minCableLength) / (cableController.maxCableLength - cableController.minCableLength));

            yield return null;
        }
    }


    void DetachAndMoveConcrete()
    {
        //detach concrete
        concrete.GetComponent<SoftParenting>().enabled = false;

        //calculate distances from crane to near and far limits
        float nearLimitDistance = Vector3.Distance(Vector3.ProjectOnPlane(crane.position, Vector3.up), Vector3.ProjectOnPlane(nearLimit.position, Vector3.up));
        float farLimitDistance = Vector3.Distance(Vector3.ProjectOnPlane(crane.position, Vector3.up), Vector3.ProjectOnPlane(farLimit.position, Vector3.up));

        Vector3 newPosition = Vector3.zero;
        bool isValidPosition = false;

        while (!isValidPosition)
        {
            //random angle for rotation
            float randomAngle = Random.Range(0f, 360f);
            Quaternion rotation = Quaternion.Euler(0, randomAngle, 0);

            //random distance within near and limits
            float randomDistance = Random.Range(nearLimitDistance, farLimitDistance);

            // calculate new position based on random angle and distance
            Vector3 direction = rotation * Vector3.forward;
            newPosition = crane.position + direction * randomDistance;

            //ensure y position is within valid range
            newPosition.y = Random.Range(10f, 20f);

            //check if new position is within valid range
            float distanceFromCrane = Vector3.Distance(Vector3.ProjectOnPlane(crane.position, Vector3.up), Vector3.ProjectOnPlane(newPosition, Vector3.up));
            if (distanceFromCrane >= nearLimitDistance && distanceFromCrane <= farLimitDistance)
            {
                isValidPosition = true;
            }
        }

        //move concrete to valid random position
        concrete.position = newPosition;

    }

}
