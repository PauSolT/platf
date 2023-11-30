using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class ChangeDimensions : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction changeDimensionAction;
    private SpriteRenderer playerSprite;
    private PlayerMovement playerMovement;

    public Transform world;
    [SerializeField]
    List<GameObject> dimensions;
    [SerializeField]
    List<TilemapCollider2D> dimensionsCollider;
    [SerializeField]
    List<Tilemap> tilemaps;

    [SerializeField]
    int currentDimension = 1;

    bool canChangeDimension = true;


    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
        changeDimensionAction = playerInput.actions["ChangeDimension"];
        for (int i = 0; i < world.childCount; i++)
        {
            dimensions.Add(world.GetChild(i).gameObject);
            dimensionsCollider.Add(dimensions[i].GetComponent<TilemapCollider2D>());
            tilemaps.Add(dimensions[i].GetComponent<Tilemap>());
        }
        ChangeDimensionHandler(1);
    }

    private void OnEnable()
    {
        changeDimensionAction.performed += ChangeDimension;

    }

    private void OnDisable()
    {
        changeDimensionAction.performed -= ChangeDimension;
    }

    private void ChangeDimension(InputAction.CallbackContext context)
    {
        if (!canChangeDimension)
            return;

        ChangeTilemapAlpha(1f);
        dimensionsCollider[currentDimension].isTrigger = false;

        float inputValue = context.ReadValue<float>();
        if (inputValue < 0)
        {
            currentDimension--;

        } else if (inputValue > 0)
        {
            currentDimension++;
            
        }
        ChangeDimensionHandler(currentDimension);
    }

    private void ChangeDimensionHandler(int dimension)
    {
        if (dimension < 1)
        {
            dimension = dimensions.Count - 1;
        }
        else if (dimension >= dimensions.Count)
        {
            dimension = 1;
        }
        currentDimension = dimension;
        dimensionsCollider[currentDimension].isTrigger = true;
        playerSprite.color = tilemaps[currentDimension].color;
        ChangeTilemapAlpha(0.4f);

        //switch (currentDimension)
        //{
        //    case 1:
        //        SwitchToRedDimension();
        //        break;
        //    case 2:
        //        SwitchToGreenDimension();
        //        break;
        //    case 3:
        //        SwitchToBlueDimension();
        //        break;
        //    default:
        //        SwitchToUniversalDimension();
        //        break;
        //}
    }

    private void SwitchToUniversalDimension()
    {
        playerMovement.Acceleration = 15f;
        playerMovement.MaxVelocity = 8f;

        playerMovement.MaxDrag = 5f;
        playerMovement.JumpForce = 10f;
    }

    private void SwitchToRedDimension()
    {
        playerMovement.Acceleration = 21f;
        playerMovement.MaxVelocity = 11f;

        playerMovement.MaxDrag = 5f;
        playerMovement.JumpForce = 10f;
    }

    private void SwitchToGreenDimension()
    {
        playerMovement.JumpForce = 13f;

        playerMovement.MaxDrag = 5f;
        playerMovement.Acceleration = 15f;
        playerMovement.MaxVelocity = 8f;
    }

    private void SwitchToBlueDimension()
    {
        playerMovement.MaxDrag = 0.5f;

        playerMovement.Acceleration = 15f;
        playerMovement.MaxVelocity = 8f;
        playerMovement.JumpForce = 10f;
    }


    private void ChangeTilemapAlpha(float alpha)
    {
        Color colorTilemap = tilemaps[currentDimension].color;
        colorTilemap.a = alpha;
        tilemaps[currentDimension].color = colorTilemap;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layer = collision.gameObject.layer;
        if(layer == 6 || layer == 7 || layer == 8)
        {
            canChangeDimension = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        int layer = collision.gameObject.layer;
        if (layer == 6 || layer == 7 || layer == 8)
        {
            canChangeDimension = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        int layer = collision.gameObject.layer;
        if (layer == 6 || layer == 7 || layer == 8)
        {
            canChangeDimension = true;
        }
    }

}
