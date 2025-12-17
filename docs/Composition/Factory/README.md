# Composition Factory

Bitflyer/Bittrade それぞれについて、標準配線を行うエントリポイントを `Composition.Factory` にまとめています。

```
using Composition.Factory;

var raw = BitflyerFactory.CreateRaw();
var adapter = BitflyerFactory.CreateAdapter(new BitflyerFactoryOptions {
    Credentials = new ApiCredentials("key","secret")
});
```
