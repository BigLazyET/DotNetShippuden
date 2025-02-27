// See https://aka.ms/new-console-template for more information

using System.Reflection;
using System.Runtime.InteropServices;

// PointerHello();
// IntPtrHello();
// SpanHello2();
// SpanArrays();
// MemoryArrays();


Console.WriteLine("Hello, World!");
Console.ReadLine();

void PointerHello()
{
    unsafe
    {
        var value = 10;
        var p = &value;
        Console.WriteLine($"{nameof(PointerHello)} p = {*p}");
        Console.WriteLine($"{nameof(PointerHello)} p intptr = {(IntPtr)p}");

        var value2 = "10";
        var p2 = &value2;
        Console.WriteLine($"{nameof(PointerHello)} p2 = {*p2}");
        Console.WriteLine($"{nameof(PointerHello)} p2 intptr = {(IntPtr)p2}");
        
        var value3 = "10";
        fixed (char* p3 = value3)
        {
            Console.WriteLine($"{nameof(PointerHello)} p3 = {*p3}");
            Console.WriteLine($"{nameof(PointerHello)} p3 intptr = {(IntPtr)p3}");
        }
    }
}

void IntPtrHello()
{
    IntPtr p = new IntPtr(10);
    Console.WriteLine($"{nameof(IntPtrHello)} p = {p}");
}

FooSpan SpanHello()
{
    // 分配栈内存
    Span<byte> span = stackalloc byte[100];

    byte data = 0;
    for (int i = 0; i < span.Length; i++)
        span[i] = data++;
    byte sum = 0;
    int sumInt = 0;
    foreach (var item in span)
    {
        sum += item;
        sumInt += item;
    }
    Console.WriteLine($"{nameof(SpanHello)} stackalloc sum byte is {sum}");  // 溢出： 4950 % 256
    Console.WriteLine($"{nameof(SpanHello)} stackalloc sum int is {sumInt}");  // 4950
    
    // 分配托管内存
    Span<byte> span2 = new Span<byte>(new byte[100]);
    byte data2 = 0;
    for (int i = 0; i < span2.Length; i++)
        span2[i] = data2++;
    
    // 分配非托管内存
    var nativeMemory = Marshal.AllocHGlobal(100);
    Span<byte> span3;
    unsafe
    {
        span3 = new Span<byte>(nativeMemory.ToPointer(), 100);
        byte data3 = 0;
        for (int i = 0; i < span3.Length; i++)
            span3[i] = data3++;
    }
    
    return new FooSpan(span2, span3, nativeMemory);
}

void SpanHello2()
{
    var fooSpan = SpanHello();
    int sum2 = 0, sum3 = 0;
    foreach (var item2 in fooSpan.Span2)
        sum2 += item2;
    foreach (var item3 in fooSpan.Span3)
        sum3 += item3;
    Console.WriteLine($"{nameof(SpanHello)} managed sum is {sum2}");
    Console.WriteLine($"{nameof(SpanHello)} unmanaged sum is {sum3}");
    
    Marshal.FreeHGlobal(fooSpan.FooIntPtr);
}

void SpanArrays()
{
    var arr1 = new[] { 1, 2, 3, 6, 7, 8, 9, 10 };
    var slice = new Span<int>(arr1, 2, 3);
    foreach (var item in slice)
    {
        Console.WriteLine($"{nameof(SpanArrays)} {item}");
    }
}

void MemoryArrays()
{
    var countries = ExtractStrings("India Belgium Australia USA UK Netherlands.".AsMemory());
    foreach (var country in countries)
    {
        Console.WriteLine(country);
    }
}

static IEnumerable<ReadOnlyMemory <char>> ExtractStrings(ReadOnlyMemory<char> c)
{
    // 异步方法和【迭代器】会被编译器转换为状态机，这些状态机的实例是分配在堆上的。
    // 限制：由于 Span<T> 是 ByRef (仅堆栈类型，仅在栈上分配) 类型 必须在栈上分配，将其用于异步方法或迭代器会导致其被分配到堆上，违反其设计原则。所以这里要用memory
    int index = 0, length = c.Length;
    for (int i = 0; i < length; i++)
    {
        if (char.IsWhiteSpace(c.Span[i]))
        {
            yield return c[index..i];
            index = i + 1;
        }
        else if (i == length - 1)
        {
            yield return c[index..];
        }
    }
}

readonly ref struct FooSpan(Span<byte> span2, Span<byte> span3, IntPtr nativeMemory)
{
    public Span<byte> Span2 { get; } = span2;
    public Span<byte> Span3 { get; } = span3;

    public IntPtr FooIntPtr { get; } = nativeMemory;
}