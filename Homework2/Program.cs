using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework2
{
  
    //Абстрактный класс доставки
    public abstract class Delivery
    {
        public int DeliveryId { get; protected set; } // Protected для доступа в наследниках
        public string RecipientAddress { get; protected set; } // Protected для доступа в наследниках
        public DateTime DeliveryDate { get; protected set; } // Protected для доступа в наследниках
        public DeliveryStatus Status { get; protected set; } // Статус доставки
        public decimal DeliveryCost { get; protected set; } // Стоимость доставки
        public Product Product { get; private set; } // Композиция с классом продукта

        // Статический счетчик для ID
        private static int _nextId = 1;

        // Свойство для получения следующего ID 
        protected static int GetNextId() { return _nextId++; }

        // Конструктор 
        public Delivery(string recipientAddress, DateTime deliveryDate, Product product)
        {
            DeliveryId = GetNextId();
            RecipientAddress = recipientAddress;
            DeliveryDate = deliveryDate;
            Status = DeliveryStatus.Pending; // Изначально доставка ожидает
            Product = product; // Композиция с продуктом
            DeliveryCost = CalculateDeliveryCost();
        }


        // Абстрактный метод для расчета стоимости доставки
        public abstract decimal CalculateDeliveryCost();

        // Виртуальный метод для получения информации о доставке
        public virtual string GetDeliveryInfo()
        {
            return $"Номер доставки: {DeliveryId}\n" +
                   $"Адрес: {RecipientAddress}\n" +
                   $"Дата: {DeliveryDate}\n" +
                   $"Статус: {Status}\n" +
                   $"Стоимость: {DeliveryCost}\n" +
                   $"Товар: {Product.Name} - {Product.Price}";
        }


        // Метод для изменения статуса доставки
        public void SetStatus(DeliveryStatus newStatus)
        {
            Status = newStatus;
        }

    }

    // Класс доставки частному лицу (наследуется от Delivery)
    public class PersonDelivery : Delivery
    {
        public string RecipientName { get; private set; } // Имя получателя

        // Конструктор 
        public PersonDelivery(string recipientName, string recipientAddress, DateTime deliveryDate, Product product) : base(recipientAddress, deliveryDate, product)
        {
            RecipientName = recipientName;
        }

        // Переопределение метода расчета стоимости доставки
        public override decimal CalculateDeliveryCost()
        {
            // Пример расчета для частного лица: базовая стоимость + зависимость от веса товара
            return 5.0m + (decimal)Product.Weight * 0.2m;
        }

        // Переопределение метода для получения информации о доставке
        public override string GetDeliveryInfo()
        {
            return base.GetDeliveryInfo() + $"\nЗаказчик: {RecipientName}";
        }

    }

    //Класс доставки в пункт выдачи (наследуется от Delivery)
    public class PickupPointDelivery : Delivery
    {
        public int PickupPointId { get; private set; } // ID пункта выдачи

        // Конструктор 
        public PickupPointDelivery(int pickupPointId, string recipientAddress, DateTime deliveryDate, Product product) : base(recipientAddress, deliveryDate, product)
        {
            PickupPointId = pickupPointId;
        }


        // Переопределение метода расчета стоимости доставки
        public override decimal CalculateDeliveryCost()
        {
            // Пример расчета для пункта выдачи: фиксированная цена
            return 2.5m;
        }

        // Переопределение метода для получения информации о доставке
        public override string GetDeliveryInfo()
        {
            return base.GetDeliveryInfo() + $"\nНомер пункта выдачи: {PickupPointId}";
        }

    }

    // 4. Класс доставки в специализированный магазин 
    public class SpecializedStoreDelivery : Delivery
    {
        public string StoreName { get; private set; } // Название магазина
        public int StoreId { get; private set; } // ID магазина

        // Конструктор с параметрами
        public SpecializedStoreDelivery(int storeId, string storeName, string recipientAddress, DateTime deliveryDate, Product product) : base(recipientAddress, deliveryDate, product)
        {
            StoreName = storeName;
            StoreId = storeId;
        }

        // Переопределение метода расчета стоимости доставки
        public override decimal CalculateDeliveryCost()
        {
            // Пример расчета для специализированного магазина: базовая стоимость + % от стоимости товара
            return 7.0m + Product.Price * 0.05m;
        }

        // Переопределение метода для получения информации о доставке
        public override string GetDeliveryInfo()
        {
            return base.GetDeliveryInfo() + $"\nНазвание магазина: {StoreName}\nНомер магазина: {StoreId}";
        }
    }

    // Товар
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public double Weight { get; set; }

        public Product(string name, decimal price, double weight)
        {
            Name = name;
            Price = price;
            Weight = weight;
        }
    }


    // Перечисление статусов доставки
    public enum DeliveryStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }

    // Обобщенный класс для работы с поставками
    public class DeliveryManager<T> where T : Delivery
    {
        private List<T> _deliveries = new List<T>();

        // Добавление доставки
        public void AddDelivery(T delivery)
        {
            _deliveries.Add(delivery);
        }

        // Поиск доставки по ID
        public T FindDelivery(int id)
        {
            return _deliveries.FirstOrDefault(delivery => delivery.DeliveryId == id);
        }

        // Удаление доставки
        public void RemoveDelivery(int id)
        {
            _deliveries.RemoveAll(delivery => delivery.DeliveryId == id);
        }

        // Получение всех доставок
        public List<T> GetDeliveries()
        {
            return _deliveries;
        }

        // Обобщенный метод для получения суммарной стоимости доставок
        public decimal GetTotalCost<TType>(List<TType> deliveries) where TType : Delivery
        {
            decimal total = 0;
            foreach (var delivery in deliveries)
            {
                total += delivery.DeliveryCost;
            }
            return total;
        }

    }


    // Метод расширения для класса Delivery 
    public static class DeliveryExtensions
    {
        public static void MarkAsCompleted(this Delivery delivery)
        {
            delivery.SetStatus(DeliveryStatus.Completed);
        }
    }

    // Наследование обобщений
    public class PersonDeliveryManager : DeliveryManager<PersonDelivery> { }

    // Агрегация 
    public class DeliveryService
    {
        public DeliveryManager<Delivery> DeliveryManager { get; private set; } = new DeliveryManager<Delivery>();


        public void AddDelivery(Delivery delivery)
        {
            DeliveryManager.AddDelivery(delivery);
        }

        public string ShowDeliveries()
        {
            string result = "";
            foreach (var delivery in DeliveryManager.GetDeliveries())
            {
                result += delivery.GetDeliveryInfo();
                result += "\n\n";
            }
            return result;
        }
    }


    // Класс с индексатором
    public class DeliveryArchive
    {
        private List<Delivery> _deliveries = new List<Delivery>();

        public void AddDelivery(Delivery delivery)
        {
            _deliveries.Add(delivery);
        }

        // Индексатор по ID доставки
        public Delivery this[int deliveryId]
        {
            get
            {
                return _deliveries.FirstOrDefault(d => d.DeliveryId == deliveryId);
            }
        }
    }

    // Перегрузка оператора
    public class ProductCollection
    {
        private List<Product> _products = new List<Product>();

        public void Add(Product product)
        {
            _products.Add(product);
        }


        public static ProductCollection operator +(ProductCollection collection, Product product)
        {
            collection._products.Add(product);
            return collection;
        }

        public string ShowProducts()
        {
            string result = "";
            foreach (var product in _products)
            {
                result += $"{product.Name} - {product.Price}\n";
            }
            return result;
        }
    }

    
    public class Program
    {
        public static void Main(string[] args)
        {
            // Создание объектов Product
            var product1 = new Product("Планшет", 1200.00m, 2.5);
            var product2 = new Product("Книга", 15.00m, 0.2);
            var product3 = new Product("Смартфон", 800.00m, 0.3);


            // Создание доставки частному лицу
            var personDelivery = new PersonDelivery("Иванов", "Яблочная, д.23, кв.14", DateTime.Now.AddDays(3), product1);
            Console.WriteLine(personDelivery.GetDeliveryInfo());

            // Создание доставки в пункт выдачи
            var pickupDelivery = new PickupPointDelivery(101, "Грушевая, 45", DateTime.Now.AddDays(2), product2);
            Console.WriteLine(pickupDelivery.GetDeliveryInfo());

            // Создание доставки в специализированный магазин
            var storeDelivery = new SpecializedStoreDelivery(200, "Техмаркет", "Вишневая, 76", DateTime.Now.AddDays(5), product3);
            Console.WriteLine(storeDelivery.GetDeliveryInfo());


            // Использование менеджера доставок
            var deliveryManager = new DeliveryManager<Delivery>();
            deliveryManager.AddDelivery(personDelivery);
            deliveryManager.AddDelivery(pickupDelivery);
            deliveryManager.AddDelivery(storeDelivery);
            Console.WriteLine("\nTotal Cost = " + deliveryManager.GetTotalCost(deliveryManager.GetDeliveries()));



            // Использование метода расширения
            personDelivery.MarkAsCompleted();
            Console.WriteLine("\nСтатус доставки после маркировки как завершенной: " + personDelivery.Status);

            // Использование менеджера для доставок PersonDelivery
            var personDeliveryManager = new PersonDeliveryManager();
            personDeliveryManager.AddDelivery(personDelivery);

            // Использование агрегации
            var deliveryService = new DeliveryService();
            deliveryService.AddDelivery(personDelivery);
            deliveryService.AddDelivery(pickupDelivery);
            deliveryService.AddDelivery(storeDelivery);
            Console.WriteLine("\nВсе доставки:\n" + deliveryService.ShowDeliveries());

            // Использование индексатора
            var archive = new DeliveryArchive();
            archive.AddDelivery(personDelivery);
            archive.AddDelivery(pickupDelivery);
            archive.AddDelivery(storeDelivery);

            Console.WriteLine("\nДоставка с номером 2:\n" + archive[2]?.GetDeliveryInfo());

            // Использование перегрузки оператора
            var products = new ProductCollection();
            products.Add(product1);
            products.Add(product2);
            products = products + product3;

            Console.WriteLine("\nВсе товары:\n" + products.ShowProducts());


            Console.ReadKey();
        }
    }
}
