using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CarMigrationMapper
{
    public static Dictionary<string, string> CarMapperDictionary = new Dictionary<string, string>()
    {
        {"car_alfaromeo_4c", "car_hyundai_Genesis_Coupe"},//t2->t4
        {"car_alfaromeo_tz3", "car_porsche_boxter"},//t4->t5
        {"car_audi_r8", "car_porsche_boxter"},//t4->t5
        {"car_audi_tt", "car_bmw_m6_coupe"},//t2->t3
        {"car_benz_a_250", "car_toyota_gt"},//t1->t2
        {"car_benz_amg_gt", "car_porsche_boxter"},//t4->t5
        {"car_bmw_i8", "car_toyota_gt"},//t3->t2
        {"car_bmw_m4", "car_bmw_m6_coupe"},//t4->t3
        {"car_bugatti_chiron", "car_bugatti_veyron"},//world tour eu win car
        {"car_chevrolet_camaro", "car_porsche_boxter"},//t2->t5
        {"car_chevrolet_corvette_c6", "car_porsche_boxter"},//t3->t5
        {"car_chevrolet_c7_z06", "car_hyundai_Genesis_Coupe"},//t5->t4
        {"car_dodge_viper_acr_x", "car_porsche_boxter"},//t4->t5
        {"car_ferrari_f430", "car_porsche_boxter"},//t4->t5
        {"car_ford_focus_rs", "car_bmw_m6_coupe"},//t2->t3
        {"car_hodna_civic_si", "car_toyota_gt"},//t1->t2
        {"car_honda_civic_type_r", "car_bmw_m6_coupe"},//t2->t3
        {"car_hyundai_Genesis_Coupe", "car_bmw_m6_coupe"},//t4->t3
        {"car_jaguar_ftype", "car_hyundai_Genesis_Coupe"},//t2->t4
        {"car_maserati_granturismo", "car_hyundai_Genesis_Coupe"},//t3->t4
        {"car_mazda_rx_type_s", "car_bmw_m6_coupe"},//t2->t3
        {"car_nissan_370_z", "car_bmw_m6_coupe"},//t2->t3
        {"car_peugeot_308_rcz", "car_bmw_m6_coupe"},//t2->t3
        {"car_porsche_boxter", "car_hyundai_Genesis_Coupe"},//t5->t4
        {"car_ford_mustang_350r", "car_hyundai_Genesis_Coupe"},//t5->t4
        {"car_tesla_model_s", "car_porsche_boxter"},//t3->t5
    };
}
