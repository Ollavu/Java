package com.company;
import java.nio.charset.IllegalCharsetNameException;
import java.util.Scanner;
import java.util.ArrayList;
import java.lang.*;
import javassist.ClassPool;

public class Main {
    static javassist.ClassPool classPool = new javassist.ClassPool().getDefault();
    public static void Err(){
        Err();
        return;
    }
    public static void main(String[] args) throws Exception {
        Scanner sc = new Scanner(System.in);
        int a = sc.nextInt();
        if(a==1)
        {
            try
            {
                int mas[]=new int[Integer.MAX_VALUE];
            }
            catch (Exception e)
            {
                System.out.println(e);
            }

        }
        if(a==2)
        {
            StringBuilder s = new StringBuilder();

                while (true) {
                    try {
                        s.append("dummy");
                    }
                    catch (Exception e)
                    {
                        System.out.println(e);
                    }
                }
        }
        if(a==3) {
            for (int i = 0; ; i++) {
                Class c = classPool.makeClass("class" + i).toClass();
                System.out.println(i);
            }
        }
        if(a==4)
        {
            Err();
        }
        if(a==5) {
            StackOverflow b = new StackOverflow();
            b.Start();
        }
    }

}
class StackOverflow {

    public void Start() {
        Class1 classOne = new Class1();
    }

    class Class1 {
        private Class2 class2;

        public Class1() {
            class2 = new Class2();
        }
    }

    class Class2 {
        private Class1 class1 = new Class1();

        public Class2() {
            class1 = new Class1();
        }
    }
}