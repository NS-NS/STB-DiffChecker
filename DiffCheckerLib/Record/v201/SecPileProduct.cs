using STBridge201;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STBDiffChecker.v201.Records
{
    internal static class SecPileProduct
    {
        internal static List<Record> Check(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB)
        {
            List<Record> records = new List<Record>();
            var secA = stBridgeA?.StbModel?.StbSections?.StbSecPileProduct;
            var secB = stBridgeB?.StbModel?.StbSections?.StbSecPileProduct;
            var setB = secB != null ? new HashSet<StbSecPileProduct>(secB) : new HashSet<StbSecPileProduct>();

            if (secA != null)
            {
                foreach (var secColumnA in secA)
                {
                    var key = new List<string>() { "Name=" + secColumnA.name };
                    var secColumnB = secB?.FirstOrDefault(n => n.name == secColumnA.name);
                    if (secColumnB != null)
                    {
                        CompareSecPileProduct(stBridgeA, stBridgeB, secColumnA, secColumnB, key, records);
                        setB.Remove(secColumnB);
                    }
                    else
                    {
                        CheckObjects.StbSecPileProduct.Compare(nameof(StbSecPileProduct), null, key, records);
                    }
                }
            }

            foreach (var b in setB)
            {
                var keyB = new List<string> { "Name=" + b.name };
                CheckObjects.StbSecPileProduct.Compare(null, nameof(StbSecPileProduct), keyB, records);
            }


            if (records.Count == 0)
                return null;
            return records;
        }

        private static void CompareSecPileProduct(ST_BRIDGE stBridgeA, ST_BRIDGE stBridgeB, StbSecPileProduct secA, StbSecPileProduct secB, IReadOnlyList<string> key, List<Record> records)
        {
            CheckObjects.StbSecPileProductId.Compare(secA.id, secB.id, key, records);
            CheckObjects.StbSecPileProductGuid.Compare(secA.guid, secB.guid, key, records);
            CheckObjects.StbSecPileProductName.Compare(secA.name, secB.name, key, records);

            CompareSecSteelFigurePileProduct(secA.StbSecFigurePileProduct, secB.StbSecFigurePileProduct, key, records);
        }

        private static void CompareSecSteelFigurePileProduct(StbSecFigurePileProduct secA, StbSecFigurePileProduct secB, IReadOnlyList<string> key, List<Record> records)
        {
            // id_orderで整理
            var pileA = SortPileProduct(secA);
            var pileB = SortPileProduct(secB);
            var order = new HashSet<int>();

            foreach (var a in pileA)
            {
                var key1 = new List<string>(key) { "id_order=" + a.Key };
                bool hasItem = false;
                foreach (var b in pileB)
                {
                    if (a.Key == b.Key)
                    {
                        CompareSecSteelFigurePileProduct(a.Value, b.Value, key1, records);
                        order.Add(a.Key);
                    }
                }

                if (!hasItem)
                {

                    CheckObjects.StbSecFigurePileProduct.Compare(GetPileProductName(a), null, key1, records);
                }
            }

            foreach (var b in pileB)
            {
                if (!order.Contains(b.Key))
                {
                    var key1 = new List<string>(key) { "id_order=" + b.Key };
                    CheckObjects.StbSecFigurePileProduct.Compare(null, GetPileProductName(b), key1, records);
                }
            }
        }

        private static void CompareSecSteelFigurePileProduct(object secA, object secB, List<string> key, List<Record> records)
        {
            if (secA is StbSecPileProduct_PHC phcA)
            {
                if (secB is StbSecPileProduct_PHC phcB)
                {
                    CheckObjects.StbSecPileProductPhcIdOrder.Compare(phcA.id_order, phcB.id_order, key, records);
                    CheckObjects.StbSecPileProductPhcProductCompany.Compare(phcA.product_company, phcB.product_company, key, records);
                    CheckObjects.StbSecPileProductPhcProductCode.Compare(phcA.product_code, phcB.product_code, key, records);
                    CheckObjects.StbSecPileProductPhcLengthPile.Compare(phcA.length_pile, phcB.length_pile, key, records);
                    CheckObjects.StbSecPileProductPhcKind.Compare(phcA.kind, phcB.kind, key, records);
                    CheckObjects.StbSecPileProductPhcD.Compare(phcA.D, phcB.D, key, records);
                    CheckObjects.StbSecPileProductPhcT.Compare(phcA.t, phcB.t, key, records);
                    CheckObjects.StbSecPileProductPhcStrengthConcrete.Compare(phcA.strength_concrete, phcB.strength_concrete, key, records);
                    CheckObjects.StbSecPileProductPhcDPc.Compare(phcA.D_PCSpecified, phcA.D_PC, phcB.D_PCSpecified,
                        phcB.D_PC, key, records);
                    CheckObjects.StbSecPileProductPhcNPc.Compare(phcA.N_PC, phcB.N_PC, key, records);
                    CheckObjects.StbSecPileProductPhcStrengthPc.Compare(phcA.strength_PC, phcB.strength_PC, key, records);
                }
                else if (secB is StbSecPileProduct_ST)
                {
                    CheckObjects.StbSecPileProductPhc.Compare(nameof(StbSecPileProduct_PHC), nameof(StbSecPileProduct_ST), key, records);
                }
                else if (secB is StbSecPileProduct_SC)
                {
                    CheckObjects.StbSecPileProductPhc.Compare(nameof(StbSecPileProduct_PHC), nameof(StbSecPileProduct_SC), key, records);
                }
                else if (secB is StbSecPileProduct_PRC)
                {
                    CheckObjects.StbSecPileProductPhc.Compare(nameof(StbSecPileProduct_PHC), nameof(StbSecPileProduct_PRC), key, records);
                }
                else if (secB is StbSecPileProduct_CPRC)
                {
                    CheckObjects.StbSecPileProductPhc.Compare(nameof(StbSecPileProduct_PHC), nameof(StbSecPileProduct_CPRC), key, records);
                }
                else if (secB is StbSecPileProductNodular_PHC)
                {
                    CheckObjects.StbSecPileProductPhc.Compare(nameof(StbSecPileProduct_PHC), nameof(StbSecPileProductNodular_PHC), key, records);
                }
                else if (secB is StbSecPileProductNodular_PRC)
                {
                    CheckObjects.StbSecPileProductPhc.Compare(nameof(StbSecPileProduct_PHC), nameof(StbSecPileProductNodular_PRC), key, records);
                }
                else if (secB is StbSecPileProductNodular_CPRC)
                {
                    CheckObjects.StbSecPileProductPhc.Compare(nameof(StbSecPileProduct_PHC), nameof(StbSecPileProductNodular_CPRC), key, records);
                }
            }
            else if (secA is StbSecPileProduct_ST stA)
            {
                if (secB is StbSecPileProduct_PHC)
                {
                    CheckObjects.StbSecPileProductSt.Compare(nameof(StbSecPileProduct_ST), nameof(StbSecPileProduct_PHC), key, records);
                }
                else if (secB is StbSecPileProduct_ST stB)
                {
                    CheckObjects.StbSecPileProductStIdOrder.Compare(stA.id_order, stB.id_order, key, records);
                    CheckObjects.StbSecPileProductStProductCompany.Compare(stA.product_company, stB.product_company, key, records);
                    CheckObjects.StbSecPileProductStProductCode.Compare(stA.product_code, stB.product_code, key, records);
                    CheckObjects.StbSecPileProductStLengthPile.Compare(stA.length_pile, stB.length_pile, key, records);
                    CheckObjects.StbSecPileProductStKind.Compare(stA.kind, stB.kind, key, records);
                    CheckObjects.StbSecPileProductStD1.Compare(stA.D1, stB.D1, key, records);
                    CheckObjects.StbSecPileProductStD2.Compare(stA.D2, stB.D2, key, records);
                    CheckObjects.StbSecPileProductStT1.Compare(stA.t1, stB.t1, key, records);
                    CheckObjects.StbSecPileProductStT2.Compare(stA.t2, stB.t2, key, records);
                    CheckObjects.StbSecPileProductStStrengthConcrete.Compare(stA.strength_concrete, stB.strength_concrete, key, records);
                    CheckObjects.StbSecPileProductStDPc.Compare(stA.D_PCSpecified, stA.D_PC, stB.D_PCSpecified,
                        stB.D_PC, key, records);
                    CheckObjects.StbSecPileProductStNPc.Compare(stA.N_PC, stB.N_PC, key, records);
                    CheckObjects.StbSecPileProductStStrengthPc.Compare(stA.strength_PC, stB.strength_PC, key, records);
                }
                else if (secB is StbSecPileProduct_SC)
                {
                    CheckObjects.StbSecPileProductSt.Compare(nameof(StbSecPileProduct_ST), nameof(StbSecPileProduct_SC), key, records);
                }
                else if (secB is StbSecPileProduct_PRC)
                {
                    CheckObjects.StbSecPileProductSt.Compare(nameof(StbSecPileProduct_ST), nameof(StbSecPileProduct_PRC), key, records);
                }
                else if (secB is StbSecPileProduct_CPRC)
                {
                    CheckObjects.StbSecPileProductSt.Compare(nameof(StbSecPileProduct_ST), nameof(StbSecPileProduct_CPRC), key, records);
                }
                else if (secB is StbSecPileProductNodular_PHC)
                {
                    CheckObjects.StbSecPileProductSt.Compare(nameof(StbSecPileProduct_ST), nameof(StbSecPileProductNodular_PHC), key, records);
                }
                else if (secB is StbSecPileProductNodular_PRC)
                {
                    CheckObjects.StbSecPileProductSt.Compare(nameof(StbSecPileProduct_ST), nameof(StbSecPileProductNodular_PRC), key, records);
                }
                else if (secB is StbSecPileProductNodular_CPRC)
                {
                    CheckObjects.StbSecPileProductSt.Compare(nameof(StbSecPileProduct_ST), nameof(StbSecPileProductNodular_CPRC), key, records);
                }
            }
            else if (secA is StbSecPileProduct_SC scA)
            {
                if (secB is StbSecPileProduct_PHC)
                {
                    CheckObjects.StbSecPileProductSc.Compare(nameof(StbSecPileProduct_SC), nameof(StbSecPileProduct_PHC), key, records);
                }
                else if (secB is StbSecPileProduct_ST)
                {
                    CheckObjects.StbSecPileProductSc.Compare(nameof(StbSecPileProduct_SC), nameof(StbSecPileProduct_ST), key, records);
                }
                else if (secB is StbSecPileProduct_SC scB)
                {
                    CheckObjects.StbSecPileProductScIdOrder.Compare(scA.id_order, scB.id_order, key, records);
                    CheckObjects.StbSecPileProductScProductCompany.Compare(scA.product_company, scB.product_company, key, records);
                    CheckObjects.StbSecPileProductScProductCode.Compare(scA.product_code, scB.product_code, key, records);
                    CheckObjects.StbSecPileProductScLengthPile.Compare(scA.length_pile, scB.length_pile, key, records);
                    CheckObjects.StbSecPileProductScKind.Compare(scA.kind, scB.kind, key, records);
                    CheckObjects.StbSecPileProductScD.Compare(scA.D, scB.D, key, records);
                    CheckObjects.StbSecPileProductScTc.Compare(scA.tc, scB.tc, key, records);
                    CheckObjects.StbSecPileProductScTs.Compare(scA.ts, scB.ts, key, records);
                    CheckObjects.StbSecPileProductScStrengthConcrete.Compare(scA.strength_concrete, scB.strength_concrete, key, records);
                    CheckObjects.StbSecPileProductScStrengthPipe.Compare(scA.strength_pipe, scB.strength_pipe, key, records);
                }
                else if (secB is StbSecPileProduct_PRC)
                {
                    CheckObjects.StbSecPileProductSc.Compare(nameof(StbSecPileProduct_SC), nameof(StbSecPileProduct_PRC), key, records);
                }
                else if (secB is StbSecPileProduct_CPRC)
                {
                    CheckObjects.StbSecPileProductSc.Compare(nameof(StbSecPileProduct_SC), nameof(StbSecPileProduct_CPRC), key, records);
                }
                else if (secB is StbSecPileProductNodular_PHC)
                {
                    CheckObjects.StbSecPileProductSc.Compare(nameof(StbSecPileProduct_SC), nameof(StbSecPileProductNodular_PHC), key, records);
                }
                else if (secB is StbSecPileProductNodular_PRC)
                {
                    CheckObjects.StbSecPileProductSc.Compare(nameof(StbSecPileProduct_SC), nameof(StbSecPileProductNodular_PRC), key, records);
                }
                else if (secB is StbSecPileProductNodular_CPRC)
                {
                    CheckObjects.StbSecPileProductSc.Compare(nameof(StbSecPileProduct_SC), nameof(StbSecPileProductNodular_CPRC), key, records);
                }
            }
            else if (secA is StbSecPileProduct_PRC prcA)
            {
                if (secB is StbSecPileProduct_PHC)
                {
                    CheckObjects.StbSecPileProductPrc.Compare(nameof(StbSecPileProduct_PRC), nameof(StbSecPileProduct_PHC), key, records);
                }
                else if (secB is StbSecPileProduct_ST)
                {
                    CheckObjects.StbSecPileProductPrc.Compare(nameof(StbSecPileProduct_PRC), nameof(StbSecPileProduct_ST), key, records);
                }
                else if (secB is StbSecPileProduct_SC)
                {
                    CheckObjects.StbSecPileProductPrc.Compare(nameof(StbSecPileProduct_PRC), nameof(StbSecPileProduct_SC), key, records);
                }
                else if (secB is StbSecPileProduct_PRC prcB)
                {
                    CheckObjects.StbSecPileProductPrcIdOrder.Compare(prcA.id_order, prcB.id_order, key, records);
                    CheckObjects.StbSecPileProductPrcProductCompany.Compare(prcA.product_company, prcB.product_company, key, records);
                    CheckObjects.StbSecPileProductPrcProductCode.Compare(prcA.product_code, prcB.product_code, key, records);
                    CheckObjects.StbSecPileProductPrcLengthPile.Compare(prcA.length_pile, prcB.length_pile, key, records);
                    CheckObjects.StbSecPileProductPrcKind.Compare(prcA.kind, prcB.kind, key, records);
                    CheckObjects.StbSecPileProductPrcD.Compare(prcA.D, prcB.D, key, records);
                    CheckObjects.StbSecPileProductPrcTc.Compare(prcA.tc, prcB.tc, key, records);
                    CheckObjects.StbSecPileProductPrcStrengthConcrete.Compare(prcA.strength_concrete, prcB.strength_concrete, key, records);
                    CheckObjects.StbSecPileProductPrcDPc.Compare(prcA.D_PC, prcB.D_PC, key, records);
                    CheckObjects.StbSecPileProductPrcNPc.Compare(prcA.N_PC, prcB.N_PC, key, records);
                    CheckObjects.StbSecPileProductPrcStrengthPc.Compare(prcA.strength_PC, prcB.strength_PC, key, records);
                    CheckObjects.StbSecPileProductPrcDBar.Compare(prcA.D_bar, prcB.D_bar, key, records);
                    CheckObjects.StbSecPileProductPrcNBar.Compare(prcA.N_bar, prcB.N_bar, key, records);
                    CheckObjects.StbSecPileProductPrcStrengthBar.Compare(prcA.strength_bar, prcB.strength_bar, key, records);
                }
                else if (secB is StbSecPileProduct_CPRC)
                {
                    CheckObjects.StbSecPileProductPrc.Compare(nameof(StbSecPileProduct_PRC), nameof(StbSecPileProduct_CPRC), key, records);
                }
                else if (secB is StbSecPileProductNodular_PHC)
                {
                    CheckObjects.StbSecPileProductPrc.Compare(nameof(StbSecPileProduct_PRC), nameof(StbSecPileProductNodular_PHC), key, records);
                }
                else if (secB is StbSecPileProductNodular_PRC)
                {
                    CheckObjects.StbSecPileProductPrc.Compare(nameof(StbSecPileProduct_PRC), nameof(StbSecPileProductNodular_PRC), key, records);
                }
                else if (secB is StbSecPileProductNodular_CPRC)
                {
                    CheckObjects.StbSecPileProductPrc.Compare(nameof(StbSecPileProduct_PRC), nameof(StbSecPileProductNodular_CPRC), key, records);
                }
            }
            else if (secA is StbSecPileProduct_CPRC cprcA)
            {
                if (secB is StbSecPileProduct_PHC)
                {
                    CheckObjects.StbSecPileProductCprc.Compare(nameof(StbSecPileProduct_CPRC), nameof(StbSecPileProduct_PHC), key, records);
                }
                else if (secB is StbSecPileProduct_ST)
                {
                    CheckObjects.StbSecPileProductCprc.Compare(nameof(StbSecPileProduct_CPRC), nameof(StbSecPileProduct_ST), key, records);
                }
                else if (secB is StbSecPileProduct_SC)
                {
                    CheckObjects.StbSecPileProductCprc.Compare(nameof(StbSecPileProduct_CPRC), nameof(StbSecPileProduct_SC), key, records);
                }
                else if (secB is StbSecPileProduct_PRC)
                {
                    CheckObjects.StbSecPileProductCprc.Compare(nameof(StbSecPileProduct_CPRC), nameof(StbSecPileProduct_PRC), key, records);
                }
                else if (secB is StbSecPileProduct_CPRC cprcB)
                {
                    CheckObjects.StbSecPileProductCprcIdOrder.Compare(cprcA.id_order, cprcB.id_order, key, records);
                    CheckObjects.StbSecPileProductCprcProductCompany.Compare(cprcA.product_company, cprcB.product_company, key, records);
                    CheckObjects.StbSecPileProductCprcProductCode.Compare(cprcA.product_code, cprcB.product_code, key, records);
                    CheckObjects.StbSecPileProductCprcLengthPile.Compare(cprcA.length_pile, cprcB.length_pile, key, records);
                    CheckObjects.StbSecPileProductCprcKind.Compare(cprcA.kind, cprcB.kind, key, records);
                    CheckObjects.StbSecPileProductCprcD.Compare(cprcA.D, cprcB.D, key, records);
                    CheckObjects.StbSecPileProductCprcTc.Compare(cprcA.tc, cprcB.tc, key, records);
                    CheckObjects.StbSecPileProductCprcStrengthConcrete.Compare(cprcA.strength_concrete, cprcB.strength_concrete, key, records);
                    CheckObjects.StbSecPileProductCprcDPc.Compare(cprcA.D_PC, cprcB.D_PC, key, records);
                    CheckObjects.StbSecPileProductCprcNPc.Compare(cprcA.N_PC, cprcB.N_PC, key, records);
                    CheckObjects.StbSecPileProductCprcStrengthPc.Compare(cprcA.strength_PC, cprcB.strength_PC, key, records);
                    CheckObjects.StbSecPileProductCprcDBar.Compare(cprcA.D_bar, cprcB.D_bar, key, records);
                    CheckObjects.StbSecPileProductCprcNBar.Compare(cprcA.N_bar, cprcB.N_bar, key, records);
                    CheckObjects.StbSecPileProductCprcStrengthBar.Compare(cprcA.strength_bar, cprcB.strength_bar, key, records);
                }
                else if (secB is StbSecPileProductNodular_PHC)
                {
                    CheckObjects.StbSecPileProductCprc.Compare(nameof(StbSecPileProduct_CPRC), nameof(StbSecPileProductNodular_PHC), key, records);
                }
                else if (secB is StbSecPileProductNodular_PRC)
                {
                    CheckObjects.StbSecPileProductCprc.Compare(nameof(StbSecPileProduct_CPRC), nameof(StbSecPileProductNodular_PRC), key, records);
                }
                else if (secB is StbSecPileProductNodular_CPRC)
                {
                    CheckObjects.StbSecPileProductCprc.Compare(nameof(StbSecPileProduct_CPRC), nameof(StbSecPileProductNodular_CPRC), key, records);
                }
            }
            else if (secA is StbSecPileProductNodular_PHC nPhcA)
            {
                if (secB is StbSecPileProduct_PHC)
                {
                    CheckObjects.StbSecPileProductNodularPhc.Compare(nameof(StbSecPileProductNodular_PHC), nameof(StbSecPileProduct_PHC), key, records);
                }
                else if (secB is StbSecPileProduct_ST)
                {
                    CheckObjects.StbSecPileProductNodularPhc.Compare(nameof(StbSecPileProductNodular_PHC), nameof(StbSecPileProduct_ST), key, records);
                }
                else if (secB is StbSecPileProduct_SC)
                {
                    CheckObjects.StbSecPileProductNodularPhc.Compare(nameof(StbSecPileProductNodular_PHC), nameof(StbSecPileProduct_SC), key, records);
                }
                else if (secB is StbSecPileProduct_PRC)
                {
                    CheckObjects.StbSecPileProductNodularPhc.Compare(nameof(StbSecPileProductNodular_PHC), nameof(StbSecPileProduct_PRC), key, records);
                }
                else if (secB is StbSecPileProduct_CPRC)
                {
                    CheckObjects.StbSecPileProductNodularPhc.Compare(nameof(StbSecPileProductNodular_PHC), nameof(StbSecPileProduct_CPRC), key, records);
                }
                else if (secB is StbSecPileProductNodular_PHC nPhcB)
                {
                    CheckObjects.StbSecPileProductNodularPhcIdOrder.Compare(nPhcA.id_order, nPhcB.id_order, key, records);
                    CheckObjects.StbSecPileProductNodularPhcProductCompany.Compare(nPhcA.product_company, nPhcB.product_company, key, records);
                    CheckObjects.StbSecPileProductNodularPhcProductCode.Compare(nPhcA.product_code, nPhcB.product_code, key, records);
                    CheckObjects.StbSecPileProductNodularPhcLengthPile.Compare(nPhcA.length_pile, nPhcB.length_pile, key, records);
                    CheckObjects.StbSecPileProductNodularPhcKind.Compare(nPhcA.kind, nPhcB.kind, key, records);
                    CheckObjects.StbSecPileProductNodularPhcD1.Compare(nPhcA.D1, nPhcB.D1, key, records);
                    CheckObjects.StbSecPileProductNodularPhcD2.Compare(nPhcA.D2, nPhcB.D2, key, records);
                    CheckObjects.StbSecPileProductNodularPhcT.Compare(nPhcA.t, nPhcB.t, key, records);
                    CheckObjects.StbSecPileProductNodularPhcStrengthConcrete.Compare(nPhcA.strength_concrete, nPhcB.strength_concrete, key, records);
                    CheckObjects.StbSecPileProductNodularPhcDPc.Compare(nPhcA.D_PC, nPhcB.D_PC, key, records);
                    CheckObjects.StbSecPileProductNodularPhcNPc.Compare(nPhcA.N_PC, nPhcB.N_PC, key, records);
                    CheckObjects.StbSecPileProductNodularPhcStrengthPc.Compare(nPhcA.strength_PC, nPhcB.strength_PC, key, records);
                }
                else if (secB is StbSecPileProductNodular_PRC)
                {
                    CheckObjects.StbSecPileProductNodularPhc.Compare(nameof(StbSecPileProductNodular_PHC), nameof(StbSecPileProductNodular_PRC), key, records);
                }
                else if (secB is StbSecPileProductNodular_CPRC)
                {
                    CheckObjects.StbSecPileProductNodularPhc.Compare(nameof(StbSecPileProductNodular_PHC), nameof(StbSecPileProductNodular_CPRC), key, records);
                }
            }
            else if (secA is StbSecPileProductNodular_PRC nPrcA)
            {
                if (secB is StbSecPileProduct_PHC)
                {
                    CheckObjects.StbSecPileProductNodularPrc.Compare(nameof(StbSecPileProductNodular_PRC), nameof(StbSecPileProduct_PHC), key, records);
                }
                else if (secB is StbSecPileProduct_ST)
                {
                    CheckObjects.StbSecPileProductNodularPrc.Compare(nameof(StbSecPileProductNodular_PRC), nameof(StbSecPileProduct_ST), key, records);
                }
                else if (secB is StbSecPileProduct_SC)
                {
                    CheckObjects.StbSecPileProductNodularPrc.Compare(nameof(StbSecPileProductNodular_PRC), nameof(StbSecPileProduct_SC), key, records);
                }
                else if (secB is StbSecPileProduct_PRC)
                {
                    CheckObjects.StbSecPileProductNodularPrc.Compare(nameof(StbSecPileProductNodular_PRC), nameof(StbSecPileProduct_PRC), key, records);
                }
                else if (secB is StbSecPileProduct_CPRC)
                {
                    CheckObjects.StbSecPileProductNodularPrc.Compare(nameof(StbSecPileProductNodular_PRC), nameof(StbSecPileProduct_CPRC), key, records);
                }
                else if (secB is StbSecPileProductNodular_PHC)
                {
                    CheckObjects.StbSecPileProductNodularPrc.Compare(nameof(StbSecPileProductNodular_PRC), nameof(StbSecPileProductNodular_PHC), key, records);
                }
                else if (secB is StbSecPileProductNodular_PRC nPrcB)
                {
                    CheckObjects.StbSecPileProductNodularPrcIdOrder.Compare(nPrcA.id_order, nPrcB.id_order, key, records);
                    CheckObjects.StbSecPileProductNodularPrcProductCompany.Compare(nPrcA.product_company, nPrcB.product_company, key, records);
                    CheckObjects.StbSecPileProductNodularPrcProductCode.Compare(nPrcA.product_code, nPrcB.product_code, key, records);
                    CheckObjects.StbSecPileProductNodularPrcLengthPile.Compare(nPrcA.length_pile, nPrcB.length_pile, key, records);
                    CheckObjects.StbSecPileProductNodularPrcKind.Compare(nPrcA.kind, nPrcB.kind, key, records);
                    CheckObjects.StbSecPileProductNodularPrcD1.Compare(nPrcA.D1, nPrcB.D1, key, records);
                    CheckObjects.StbSecPileProductNodularPrcD2.Compare(nPrcA.D2, nPrcB.D2, key, records);
                    CheckObjects.StbSecPileProductNodularPrcTc.Compare(nPrcA.tc, nPrcB.tc, key, records);
                    CheckObjects.StbSecPileProductNodularPrcStrengthConcrete.Compare(nPrcA.strength_concrete, nPrcB.strength_concrete, key, records);
                    CheckObjects.StbSecPileProductNodularPrcDPc.Compare(nPrcA.D_PC, nPrcB.D_PC, key, records);
                    CheckObjects.StbSecPileProductNodularPrcNPc.Compare(nPrcA.N_PC, nPrcB.N_PC, key, records);
                    CheckObjects.StbSecPileProductNodularPrcStrengthPc.Compare(nPrcA.strength_PC, nPrcB.strength_PC, key, records);
                    CheckObjects.StbSecPileProductNodularPrcDBar.Compare(nPrcA.D_bar, nPrcB.D_bar, key, records);
                    CheckObjects.StbSecPileProductNodularPrcNBar.Compare(nPrcA.N_bar, nPrcB.N_bar, key, records);
                    CheckObjects.StbSecPileProductNodularPrcStrengthBar.Compare(nPrcA.strength_bar, nPrcB.strength_bar, key, records);
                }
                else if (secB is StbSecPileProductNodular_CPRC)
                {
                    CheckObjects.StbSecPileProductNodularPrc.Compare(nameof(StbSecPileProductNodular_PRC), nameof(StbSecPileProductNodular_CPRC), key, records);
                }
            }
            else if (secA is StbSecPileProductNodular_CPRC nCprcA)
            {
                if (secB is StbSecPileProduct_PHC)
                {
                    CheckObjects.StbSecPileProductNodularCprc.Compare(nameof(StbSecPileProductNodular_CPRC), nameof(StbSecPileProduct_PHC), key, records);
                }
                else if (secB is StbSecPileProduct_ST)
                {
                    CheckObjects.StbSecPileProductNodularCprc.Compare(nameof(StbSecPileProductNodular_CPRC), nameof(StbSecPileProduct_ST), key, records);
                }
                else if (secB is StbSecPileProduct_SC)
                {
                    CheckObjects.StbSecPileProductNodularCprc.Compare(nameof(StbSecPileProductNodular_CPRC), nameof(StbSecPileProduct_SC), key, records);
                }
                else if (secB is StbSecPileProduct_PRC)
                {
                    CheckObjects.StbSecPileProductNodularCprc.Compare(nameof(StbSecPileProductNodular_CPRC), nameof(StbSecPileProduct_PRC), key, records);
                }
                else if (secB is StbSecPileProduct_CPRC)
                {
                    CheckObjects.StbSecPileProductNodularCprc.Compare(nameof(StbSecPileProductNodular_CPRC), nameof(StbSecPileProduct_CPRC), key, records);
                }
                else if (secB is StbSecPileProductNodular_PHC)
                {
                    CheckObjects.StbSecPileProductNodularCprc.Compare(nameof(StbSecPileProductNodular_CPRC), nameof(StbSecPileProductNodular_PHC), key, records);
                }
                else if (secB is StbSecPileProductNodular_PRC)
                {
                    CheckObjects.StbSecPileProductNodularCprc.Compare(nameof(StbSecPileProductNodular_CPRC), nameof(StbSecPileProductNodular_PRC), key, records);
                }
                else if (secB is StbSecPileProductNodular_CPRC nCprcB)
                {
                    CheckObjects.StbSecPileProductNodularCprcIdOrder.Compare(nCprcA.id_order, nCprcB.id_order, key, records);
                    CheckObjects.StbSecPileProductNodularCprcProductCompany.Compare(nCprcA.product_company, nCprcB.product_company, key, records);
                    CheckObjects.StbSecPileProductNodularCprcProductCode.Compare(nCprcA.product_code, nCprcB.product_code, key, records);
                    CheckObjects.StbSecPileProductNodularCprcLengthPile.Compare(nCprcA.length_pile, nCprcB.length_pile, key, records);
                    CheckObjects.StbSecPileProductNodularCprcKind.Compare(nCprcA.kind, nCprcB.kind, key, records);
                    CheckObjects.StbSecPileProductNodularCprcD1.Compare(nCprcA.D1, nCprcB.D1, key, records);
                    CheckObjects.StbSecPileProductNodularCprcD2.Compare(nCprcA.D2, nCprcB.D2, key, records);
                    CheckObjects.StbSecPileProductNodularCprcTc.Compare(nCprcA.tc, nCprcB.tc, key, records);
                    CheckObjects.StbSecPileProductNodularCprcStrengthConcrete.Compare(nCprcA.strength_concrete, nCprcB.strength_concrete, key, records);
                    CheckObjects.StbSecPileProductNodularCprcDPc.Compare(nCprcA.D_PC, nCprcB.D_PC, key, records);
                    CheckObjects.StbSecPileProductNodularCprcNPc.Compare(nCprcA.N_PC, nCprcB.N_PC, key, records);
                    CheckObjects.StbSecPileProductNodularCprcStrengthPc.Compare(nCprcA.strength_PC, nCprcB.strength_PC, key, records);
                    CheckObjects.StbSecPileProductNodularCprcDBar.Compare(nCprcA.D_bar, nCprcB.D_bar, key, records);
                    CheckObjects.StbSecPileProductNodularCprcNBar.Compare(nCprcA.N_bar, nCprcB.N_bar, key, records);
                    CheckObjects.StbSecPileProductNodularCprcStrengthBar.Compare(nCprcA.strength_bar, nCprcB.strength_bar, key, records);
                }
            }
        }

        private static string GetPileProductName(object obj)
        {
            if (obj is StbSecPileProduct_PHC)
                return nameof(StbSecPileProduct_PHC);
            else if (obj is StbSecPileProduct_ST)
                return nameof(StbSecPileProduct_ST);
            else if (obj is StbSecPileProduct_SC)
                return nameof(StbSecPileProduct_SC);
            else if (obj is StbSecPileProduct_PRC)
                return nameof(StbSecPileProduct_PRC);
            else if (obj is StbSecPileProduct_CPRC)
                return nameof(StbSecPileProduct_CPRC);
            else if (obj is StbSecPileProductNodular_PHC)
                return nameof(StbSecPileProductNodular_PHC);
            else if (obj is StbSecPileProductNodular_PRC)
                return nameof(StbSecPileProductNodular_PRC);
            else if (obj is StbSecPileProductNodular_CPRC)
                return nameof(StbSecPileProductNodular_CPRC);
            throw new Exception();
        }

        private static Dictionary<int, object> SortPileProduct(StbSecFigurePileProduct secA)
        {
            var pile = new Dictionary<int, object>();
            if (secA.StbSecPileProduct_PHC != null)
            {
                foreach (var straight in secA.StbSecPileProduct_PHC)
                {
                    pile.Add(int.Parse(straight.id_order), straight);
                }
            }

            if (secA.StbSecPileProduct_ST != null)
            {
                foreach (var rotational in secA.StbSecPileProduct_ST)
                {
                    pile.Add(int.Parse(rotational.id_order), rotational);
                }
            }

            if (secA.StbSecPileProduct_SC != null)
            {
                foreach (var taper in secA.StbSecPileProduct_SC)
                {
                    pile.Add(int.Parse(taper.id_order), taper);
                }
            }

            if (secA.StbSecPileProduct_PRC != null)
            {
                foreach (var taper in secA.StbSecPileProduct_PRC)
                {
                    pile.Add(int.Parse(taper.id_order), taper);
                }
            }

            if (secA.StbSecPileProduct_CPRC != null)
            {
                foreach (var taper in secA.StbSecPileProduct_CPRC)
                {
                    pile.Add(int.Parse(taper.id_order), taper);
                }
            }

            if (secA.StbSecPileProductNodular_PHC != null)
            {
                foreach (var taper in secA.StbSecPileProductNodular_PHC)
                {
                    pile.Add(int.Parse(taper.id_order), taper);
                }
            }

            if (secA.StbSecPileProductNodular_PRC != null)
            {
                foreach (var taper in secA.StbSecPileProductNodular_PRC)
                {
                    pile.Add(int.Parse(taper.id_order), taper);
                }
            }

            if (secA.StbSecPileProductNodular_CPRC != null)
            {
                foreach (var taper in secA.StbSecPileProductNodular_CPRC)
                {
                    pile.Add(int.Parse(taper.id_order), taper);
                }
            }
            return pile;
        }
    }
}
