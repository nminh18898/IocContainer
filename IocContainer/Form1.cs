using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IocContainer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var container = new IocContainer();

            container.Register<ICar, Audi>();

            var a = container.Resolve<ICar>(); // a là object kiểu Audi

            Driver drv = container.Resolve<Driver>(); // drv là kiểu Driver, trong đó có thuộc tính ICar là Audi => Constructor Injection (class Driver phải có constructor với 1 tham số là ICar)
            OtherDriver otherDrv = container.Resolve<OtherDriver>(); // otherDrv là kiểu OtherDriver,  trong đó có thuộc tính ICar là Audi => Property Injection => Phải có phương thức để set ICar, là 1 constructor ko tham số (để tạo được object)

            drv.RunCar();
            otherDrv.RunCar();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public interface ICar
        {
            int Run();
        }

        public class BMW : ICar
        {
            private int _miles = 0;

            public int Run()
            {
                return ++_miles;
            }
        }

        public class Ford : ICar
        {
            private int _miles = 0;

            public int Run()
            {
                return ++_miles;
            }
        }

        public class Audi : ICar
        {
            private int _miles = 0;

            public int Run()
            {
                return ++_miles;
            }

        }

        public class Driver
        {
            private ICar _car = null;

            public Driver()
            {

            }


            public Driver(ICar car)
            {
                _car = car;
            }

            public Driver(int a, int b)
            {
                
            }

            public void RunCar()
            {
                Console.WriteLine("Running {0} - {1} mile ", _car.GetType().Name, _car.Run());
            }

        }


        public class OtherDriver
        {
            private ICar _car = null;

            public OtherDriver()
            {

            }

            public void RunCar()
            {
                Console.WriteLine("Running {0} - {1} mile ", _car.GetType().Name, _car.Run());
            }

            [IocAttributes]
            public void setCar(ICar car)
            {
                _car = car;
            }
        }
    }


    
}

