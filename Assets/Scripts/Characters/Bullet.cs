using UnityEngine;

public class Bullet : MonoBehaviour
{
   [SerializeField] public float speed;
   [SerializeField] public int damage;
    public int team; // ใช้เพื่อตรวจสอบว่ากระสุนมาจากทีมไหน
    private Vector2 direction;
    private bool isTargetAcquired = false; // ตัวแปรใหม่ที่ใช้ตรวจสอบว่าเจอเป้าหมายหรือยัง

    private void Start()
    {
        Destroy(gameObject, 5f); // ลบกระสุนออกจากเกมหลังจาก 5 วินาที
    }

    private void Update()
    {
        // ค้นหาศัตรูในระยะใกล้ๆ (ในกรณีนี้ 10f)
        if (!isTargetAcquired) 
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10f); // ค้นหาในระยะ 10f
            foreach (var enemy in enemies)
            {
                Unit targetUnit = enemy.GetComponent<Unit>();
                Building targetBuilding = enemy.GetComponent<Building>();
                Base targetHeadquarters = enemy.GetComponent<Base>(); // ตรวจสอบกับฐาน

                // ถ้าเจอยูนิตของฝ่ายตรงข้าม
                if (targetUnit != null && targetUnit.team != team)
                {
                    // คำนวณทิศทางจากตัวกระสุนไปยังศัตรู
                    direction = (targetUnit.transform.position - transform.position).normalized;
                    // หมุนกระสุนให้หันไปทางศัตรู
                    RotateBullet();
                    isTargetAcquired = true; // เปลี่ยนสถานะว่าพบเป้าหมายแล้ว
                    break; // หยุดการตรวจสอบเมื่อเจอเป้าหมาย
                }

                // ถ้าชนอาคารของฝ่ายตรงข้าม
                if (targetBuilding != null && targetBuilding.team != team)
                {
                    // คำนวณทิศทางจากตัวกระสุนไปยังอาคาร
                    direction = (targetBuilding.transform.position - transform.position).normalized;
                    // หมุนกระสุนให้หันไปทางอาคาร
                    RotateBullet();
                    isTargetAcquired = true; // เปลี่ยนสถานะว่าพบเป้าหมายแล้ว
                    break; // หยุดการตรวจสอบเมื่อเจอเป้าหมาย
                }

                // ถ้าชนกับ Headquarters (ฐาน)
                if (targetHeadquarters != null && targetHeadquarters.team != team)
                {
                    // คำนวณทิศทางจากตัวกระสุนไปยังฐาน
                    direction = (targetHeadquarters.transform.position - transform.position).normalized;
                    // หมุนกระสุนให้หันไปทางฐาน
                    RotateBullet();
                    isTargetAcquired = true; // เปลี่ยนสถานะว่าพบเป้าหมายแล้ว
                    break; // หยุดการตรวจสอบเมื่อเจอเป้าหมาย
                }
            }
        }

        // ถ้าเจอเป้าหมายแล้วให้กระสุนเคลื่อนที่ไปหามัน
        if (isTargetAcquired)
        {
            MoveBullet();
        }
    }

    private void RotateBullet()
    {
        // หมุนกระสุนให้หันไปทางเป้าหมาย
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void MoveBullet()
    {
        // เคลื่อนที่กระสุนในทิศทางที่ยูนิตหันไป
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ตรวจสอบว่ากระสุนชนกับเกมอ็อบเจ็กต์ที่มี Tag เป็น "Unit"
        if (collision.CompareTag("Unit"))
        {
            Unit targetUnit = collision.GetComponent<Unit>();
            if (targetUnit != null && targetUnit.team != team)  // ตรวจสอบฝ่ายของยูนิต
            {
                targetUnit.TakeDamage(damage);  // ส่งความเสียหายให้ยูนิต
                Destroy(gameObject);  // ลบกระสุน
            }
        }

        // ตรวจสอบอาคาร (ถ้าต้องการให้กระสุนชนกับอาคาร)
        Building targetBuilding = collision.gameObject.GetComponent<Building>();
        if (targetBuilding != null && targetBuilding.team != team)
        {
            targetBuilding.TakeDamage(damage);
            Destroy(gameObject);
        }

        // ตรวจสอบการชนกับ Headquarters
        Base targetHeadquarters = collision.gameObject.GetComponent<Base>();
        if (targetHeadquarters != null && targetHeadquarters.team != team)
        {
            targetHeadquarters.TakeDamage(damage); // ลด HP ของฐาน
            Destroy(gameObject);
        }
    }
}
