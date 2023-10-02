using m4dHART;
using m4dHART._2_DataLinkLayer.Wired_Token_Passing;

namespace m4dHARTTests
{
    [TestClass]
    public class Token_Passing_PDU_Tests
    {
        [TestMethod]
        public void CheckByteTest()
        {
            var Delimiter = new Delimiter(AddressType.Polling, 0, m4dHART._2_DataLinkLayer.PhysicalLayerType.Asyncronous, FrameType.STX);
            var pdu = new Token_Passing_PDU(Delimiter, new ShortAddress(0), new byte[0], 0, new byte[0]);
            Assert.AreEqual(2, pdu.CheckByte);
            Assert.IsTrue(pdu.IsCorrect());

        }
        [TestMethod]
        public void CheckByteTest2()
        {
            var Delimiter = new Delimiter(AddressType.Unique, 0, m4dHART._2_DataLinkLayer.PhysicalLayerType.Asyncronous, FrameType.STX);
            var pdu = new Token_Passing_PDU(Delimiter, new LongAddress(new byte[] {0x37, 0x0b, 0xa8, 0xab, 0x50}), new byte[0], 13, new byte[0]);
            Assert.AreEqual(0xe0, pdu.CheckByte);
            Assert.IsTrue(pdu.IsCorrect());
        }
    }
}
