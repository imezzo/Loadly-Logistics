namespace LoadlyAggregator.Infrastructure.Data.ACL;



public static class AdminAcl
{
    public static Dictionary<string, List<string>> Permissions => new()
    {
        ["admins"] = new List<string>
        {
            "view_admin",
            "create_admin",
            "edit_admin",
            "delete_admin",
        },
        ["collectors"] = new List<string>
        {
            "view_collector",
            "create_collector",
            "edit_collector",
            "delete_collector",
        },
        ["activity_logs"] = new List<string>
        {
            "view_activity_log",
        },
        ["notifications_logs"] = new List<string>
        {
            "view_notifications_log",
        },
        ["odometer_scan_logs"] = new List<string>
        {
            "view_odometer_scan_log",
        },
        ["permissions"] = new List<string>
        {
            "manage_permissions",
        },
        ["addresses"] = new List<string>
        {
            "view_city",
            "create_city",
            "edit_city",
            "delete_city",
            "view_governerate",
            "create_governerate",
            "edit_governerate",
            "delete_governerate",
            "view_district",
            "create_district",
            "edit_district",
            "delete_district",
        },
        ["industry_types"] = new List<string>
        {
            "view_industry_type",
            "create_industry_type",
            "edit_industry_type",
            "delete_industry_type",
        },
        ["sectors"] = new List<string>
        {
            "view_sector",
            "create_sector",
            "edit_sector",
            "delete_sector",
        },
        ["communication_methods"] = new List<string>
        {
            "view_communication_method",
            "create_communication_method",
            "edit_communication_method",
            "delete_communication_method",
        },
        ["operating_hours"] = new List<string>
        {
            "view_operating_hour",
            "create_operating_hour",
            "edit_operating_hour",
            "delete_operating_hour",
        },
        ["service_requirements"] = new List<string>
        {
            "view_service_requirement",
            "create_service_requirement",
            "edit_service_requirement",
            "delete_service_requirement",
        },
        ["documents"] = new List<string>
        {
            "view_document",
            "create_document",
            "edit_document",
            "delete_document",
        },
        ["companies"] = new List<string>
        {
            "view_company",
            "create_company",
            "edit_company",
            "delete_company",
        },
        ["vehicle_types"] = new List<string>
        {
            "view_vehicle_type",
            "create_vehicle_type",
            "edit_vehicle_type",
            "delete_vehicle_type",
        },
        ["taxes"] = new List<string>
        {
            "view_tax",
            "create_tax",
            "edit_tax",
            "delete_tax",
        },
        ["leads"] = new List<string>
        {
            "view_lead",
            "create_lead",
            "edit_lead",
            "delete_lead",
        },
        ["discounts"] = new List<string>
        {
            "view_discount",
            "create_discount",
            "edit_discount",
            "delete_discount",
        },
        ["colors"] = new List<string>
        {
            "view_color",
            "create_color",
            "edit_color",
            "delete_color",
        },
        ["tags"] = new List<string>
        {
            "view_tag",
            "create_tag",
            "edit_tag",
            "delete_tag",
        },
        ["fuel_types"] = new List<string>
        {
            "view_fuel_type",
            "create_fuel_type",
            "edit_fuel_type",
            "delete_fuel_type",
        },
        ["contracts"] = new List<string>
        {
            "view_contract",
            "create_contract",
            "edit_contract",
            "edit_contract:active",
            "delete_contract",
            "view_slot_activity",
        },
        ["contract_billing_snapshots"] = new List<string>
        {
            "view_contract_billing_snapshot",
            "create_contract_billing_snapshot",
            "edit_contract_billing_snapshot",
            "delete_contract_billing_snapshot",
        },
        ["contract_management"] = new List<string>
        {
            "pull_back_to_draft_contract",
            "deactivate_contract",
            "reactivate_contract",
            "terminate_contract",
        },
        ["contract_approvals"] = new List<string>
        {
            "approve_contract_sales",
            "approve_contract_finance",
            "approve_contract_operations",
        },
        ["trips"] = new List<string>
        {
            "view_trip",
            "create_trip",
            "create_historical_trip",
            "edit_trip",
            "edit_trip:in_progress",
            "edit_trip:completed",
            "delete_trip",
            "cancel_trip",
            "cancel_trip:in_progress",
            "complete_trip",
            "self_complete_trip",
            "edit_charges",
            "audit_trip",
            "pick_other_contract_vehicle",
        },
        ["vehicle_capacity_units"] = new List<string>
        {
            "view_vehicle_capacity_unit",
            "create_vehicle_capacity_unit",
            "edit_vehicle_capacity_unit",
            "delete_vehicle_capacity_unit",
        },
        ["ports"] = new List<string>
        {
            "view_port",
            "create_port",
            "edit_port",
            "delete_port",
        },
        ["containers"] = new List<string>
        {
            "view_container",
            "create_container",
            "edit_container",
            "delete_container",
        },
        ["shipping_lines"] = new List<string>
        {
            "view_shipping_line",
            "create_shipping_line",
            "edit_shipping_line",
            "delete_shipping_line",
        },
        ["vehicles"] = new List<string>
        {
            "view_vehicle",
            "create_vehicle",
            "edit_vehicle",
            "assign_driver",
            "delete_vehicle",
        },
        ["vehicle_insurances"] = new List<string>
        {
            "view_vehicle_insurance",
            "create_vehicle_insurance",
            "edit_vehicle_insurance",
            "delete_vehicle_insurance",
        },
        ["vehicle_facilities"] = new List<string>
        {
            "view_vehicle_facility",
            "create_vehicle_facility",
            "edit_vehicle_facility",
            "delete_vehicle_facility",
        },
        ["vehicle_facility_visits"] = new List<string>
        {
            "enter_vehicle_facility_visit",
            "exit_vehicle_facility_visit",
        },
        ["vehicle_brands"] = new List<string>
        {
            "view_vehicle_brand",
            "create_vehicle_brand",
            "edit_vehicle_brand",
            "delete_vehicle_brand",
        },
        ["vehicle_brand_models"] = new List<string>
        {
            "view_vehicle_brand_model",
            "create_vehicle_brand_model",
            "edit_vehicle_brand_model",
            "delete_vehicle_brand_model",
        },
        ["driver_license_types"] = new List<string>
        {
            "view_driver_license_type",
            "create_driver_license_type",
            "edit_driver_license_type",
            "delete_driver_license_type",
        },
        ["driver_documents"] = new List<string>
        {
            "view_driver_document",
            "create_driver_document",
            "edit_driver_document",
            "delete_driver_document",
        },
        ["drivers"] = new List<string>
        {
            "view_driver",
            "create_driver",
            "edit_driver",
            "delete_driver",
        },
        ["contractors"] = new List<string>
        {
            "view_contractor",
            "create_contractor",
            "edit_contractor",
            "delete_contractor",
        },
        ["profile"] = new List<string>
        {
            "view_profile",
            "edit_profile",
        },
        ["push_notifications"] = new List<string>
        {
            "send_push_notification",
        }
        ,
        ["broadcast_notifications"] = new List<string>
        {
            "view_broadcast_notification",
            "send_broadcast_notification",
        },
        ["invoice"] = new List<string>
        {
            "view_invoice",
            "create_invoice",
            "cancel_invoice",
        },
        ["bank_transfers"] = new List<string>
        {
            "view_bank_transfer",
            "create_bank_transfer",
            "edit_bank_transfer",
            "delete_bank_transfer",
        },
        ["contractor_payouts"] = new List<string>
        {
            "view_contractor_payout",
            "create_contractor_payout",
            "edit_contractor_payout",
        },
        ["dashboard"] = new List<string>
        {
            "view_dashboard",
            "manage_dashboard_targets",
        },
        ["settings"] = new List<string>
        {
            "view_settings",
            "edit_settings",
        }
    };
}
