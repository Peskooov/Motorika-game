﻿using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private AbilitiesChanger abilitiesChanger;
    [SerializeField] private TurretMode mode;
    public TurretMode Mode => mode;

    [SerializeField] private TurretProperties turretProperties;

    [SerializeField] private TurretProperties[] AllTurrets;

    private float refireTimer; // Cooldown
    public bool CanFire => refireTimer <= 0; // State of Cooldown

    private Player player; // Parent object
    private Destructible destructable; // Parent object

    /*[SerializeField] private UpgradeAsset countTurretUpgrade;
    private int levelOfTurret;

    private void Awake()
    {
        if (countTurretUpgrade)
            levelOfTurret = Upgrades.GetUpgradeLevel(countTurretUpgrade);
    } */

    private void Start()
    {
        destructable = transform.root.GetComponent<Destructible>();

        PlayerInputController.Instance.FirstAbilityEvent += Fire;
    }

    private void OnDestroy()
    {
        PlayerInputController.Instance.FirstAbilityEvent -= Fire;
    }

    private void Update()
    {
        
        if (refireTimer > 0)
            refireTimer -= Time.deltaTime;

        //else if (Mode == TurretMode.Auto)
        //    Fire();
    }

    public void Fire()
    {
        if (refireTimer > 0)
            return;

        if (turretProperties == null)
            return;

        if (player)
        {
            if (!player.DrawEnergy(turretProperties.EnergyUsage))
                return;

            if (!player.DrawAmmo(turretProperties.AmmoUsage))
                return;
        }

        if (abilitiesChanger && AllTurrets.Length > 0)
        {
            if (abilitiesChanger.PreviousFirstIndex != 3 || abilitiesChanger.PreviousFirstIndex != 4 ||
                abilitiesChanger.PreviousFirstIndex != 5)
            {
                mode = TurretMode.Null;
                turretProperties = AllTurrets[3];
            }

            if (abilitiesChanger.PreviousFirstIndex == 3)
            {
                mode = TurretMode.Lightning;
                turretProperties = AllTurrets[0];
            }

            if (abilitiesChanger.PreviousFirstIndex == 4)
            {
                mode = TurretMode.Freezing;
                turretProperties = AllTurrets[1];
            }

            if (abilitiesChanger.PreviousFirstIndex == 5)
            {
                mode = TurretMode.AutoAiming;
                turretProperties = AllTurrets[2];
            }
           
        }
        CreateLightningProjectille();
        CreateAimingProjectille();
        CreateFreezingProjectille();
       
    
        refireTimer = turretProperties.RateOfFire;

        {
            // SFX
        }
    
        
    }
    public void EnemyFire()
    {
        if (refireTimer > 0)
            return;

        if (turretProperties == null)
            return;

    

           
        CreateEnemyProjectile();
           
        refireTimer = turretProperties.RateOfFire;


       
    }
    public void AssignLoadout(TurretProperties props)
    {
        if (props == null)
        {
            // Логгирование или обработка случая, когда props равен null
            return;
        }

        if (mode != props.Mode)
        {
            // Логгирование или уведомление о несоответствии режимов
            return;
        }

        lock (this)
        {
            turretProperties = props;
            refireTimer = 0;
        }
    }

    private void CreateEnemyProjectile()
    {
        if (mode != TurretMode.Enemy) return;

        var projectile = Instantiate(turretProperties.ProjectilePrefab.gameObject).GetComponent<Projectile>();
        projectile.transform.position = transform.position;
        projectile.transform.forward = transform.forward;

        projectile.SetParentShooter(destructable);
        projectile.EnemyProjectile();
    }
    private void CreateLightningProjectille()
    {
        if (mode != TurretMode.Lightning) return;
        AchievementManager.Instance.LightningWeapon();
        var projectile = Instantiate(turretProperties.ProjectilePrefab.gameObject).GetComponent<Projectile>();
        projectile.transform.position = transform.position;
        projectile.transform.forward = transform.forward;

        projectile.SetParentShooter(destructable);
        projectile.LightningProjectile();
    }

    private void CreateFreezingProjectille()
    {
        if (mode != TurretMode.Freezing) return;
        AchievementManager.Instance.FreezeWeapon();
        var projectile = Instantiate(turretProperties.ProjectilePrefab.gameObject).GetComponent<Projectile>();
        projectile.transform.position = transform.position;
        projectile.transform.forward = transform.forward;

        projectile.SetParentShooter(destructable);
        projectile.FreezingProjectile();
    }

    private void CreateAimingProjectille()
    {
        if (mode != TurretMode.AutoAiming) return;
        AchievementManager.Instance.AimWeapon();
        var projectile = Instantiate(turretProperties.ProjectilePrefab.gameObject).GetComponent<Projectile>();
        projectile.transform.position = transform.position;
        projectile.transform.forward = transform.forward;

        projectile.SetParentShooter(destructable);
        projectile.AimingProjectile();
    }
}