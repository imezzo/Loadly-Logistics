export const Permission = {
  ActivityLog: 'activity_logs',
  Address: 'addresses',
  AddressManagement: 'addresses_management',
  Admin: 'admins',
  Collector: 'collectors',
  Color: 'colors',
  CommunicationMethod: 'communication_methods',
  Company: 'companies',
  CompanyDocument: 'company_documents',
  Contact: 'contacts',
  Contract: 'contracts',
  ContractApprovals: 'contract_approvals',
  ContractBillingSnapshot: 'contract_billing_snapshots',
  ContractDocument: 'contract_documents',
  ContractFuelType: 'contract_fuel_types',
  ContractManagement: 'contract_management',
  ContractType: 'contract_types',
  ContractVehicleType: 'contract_vehicle_types',
  Contractor: 'contractors',
  Dashboard: 'dashboard',
  Discount: 'discounts',
  Document: 'documents',
  Driver: 'drivers',
  DriverDocument: 'driver_documents',
  DriverLicenseType: 'driver_license_types',
  FuelType: 'fuel_types',
  IndustryType: 'industry_types',
  Invoice: 'invoice',
  Lead: 'leads',
  Miscelleanous: 'miscelleanous',
  ModelHasPermission: 'model_has_permissions',
  ModelHasRole: 'model_has_roles',
  NotificationLog: 'notifications_logs',
  PermissionModule: 'permissions',
  Role: 'roles',
  RoleHasPermission: 'role_has_permissions',
  Sector: 'sectors',
  ServiceRequirement: 'service_requirements',
  Tax: 'taxes',
  Trip: 'trips',
  Users: 'users',
  Vehicle: 'vehicles',
  VehicleBrand: 'vehicle_brands',
  VehicleBrandModel: 'vehicle_brand_models',
  VehicleCapacityUnit: 'vehicle_capacity_units',
  VehicleChangeRequest: 'vehicle_change_requests',
  VehicleFacility: 'vehicle_facilities',
  VehicleFacilityVisit: 'vehicle_facility_visits',
  VehicleInsurance: 'vehicle_insurances',
  VehicleType: 'vehicle_types',
} as const

export const Miscelleanous = {
  BulkUpdateStatus: 'admins.bulk_update_status',
} as const

export const Contract = {
  Create: 'create_contract',
  Delete: 'delete_contract',
  Edit: 'edit_contract',
  EditActive: 'edit_contract:active',
  View: 'view_contract',
  ViewSlotActivity: 'view_slot_activity',
} as const

export const ContractType = {
  Create: 'create_contract_type',
  Delete: 'delete_contract_type',
  Edit: 'edit_contract_type',
  View: 'view_contract_type',
} as const

export const ContractManagement = {
  PullBackToDraft: 'pull_back_to_draft_contract',
  Deactivate: 'deactivate_contract',
  Terminate: 'terminate_contract',
} as const

export const AddressManagement = {
  Create: 'create_address',
  Delete: 'delete_address',
  Edit: 'edit_address',
  View: 'view_address',
} as const

export const Admin = {
  Create: 'create_admin',
  Delete: 'delete_admin',
  Edit: 'edit_admin',
  View: 'view_admin',
} as const

export const Address = {
  CreateCity: 'create_city',
  CreateDistrict: 'create_district',
  CreateGovernerate: 'create_governerate',
  DeleteCity: 'delete_city',
  DeleteDistrict: 'delete_district',
  DeleteGovernerate: 'delete_governerate',
  EditCity: 'edit_city',
  EditDistrict: 'edit_district',
  EditGovernerate: 'edit_governerate',
  ViewCity: 'view_city',
  ViewDistrict: 'view_district',
  ViewGovernerate: 'view_governerate',
} as const

export const Collector = {
  Create: 'create_collector',
  Delete: 'delete_collector',
  Edit: 'edit_collector',
  View: 'view_collector',
} as const

export const Color = {
  Create: 'create_color',
  Delete: 'delete_color',
  Edit: 'edit_color',
  View: 'view_color',
} as const

export const CommunicationMethod = {
  Create: 'create_communication_method',
  Delete: 'delete_communication_method',
  Edit: 'edit_communication_method',
  View: 'view_communication_method',
} as const

export const Company = {
  Create: 'create_company',
  Delete: 'delete_company',
  Edit: 'edit_company',
  View: 'view_company',
} as const

export const CompanyDocument = {
  Create: 'create_company_document',
  Delete: 'delete_company_document',
  Edit: 'edit_company_document',
  View: 'view_company_document',
} as const

export const Contact = {
  Create: 'create_contact',
  Delete: 'delete_contact',
  Edit: 'edit_contact',
  View: 'view_contact',
} as const

export const ContractApprovals = {
  Finance: 'approve_contract_finance',
  Operations: 'approve_contract_operations',
  Sales: 'approve_contract_sales',
} as const

export const ContractBillingSnapshot = {
  Create: 'create_contract_billing_snapshot',
  Delete: 'delete_contract_billing_snapshot',
  Edit: 'edit_contract_billing_snapshot',
  View: 'view_contract_billing_snapshot',
} as const

export const ContractDocument = {
  Create: 'create_contract_document',
  Delete: 'delete_contract_document',
  Edit: 'edit_contract_document',
  View: 'view_contract_document',
} as const

export const ContractFuelType = {
  Create: 'create_contract_fuel_type',
  Delete: 'delete_contract_fuel_type',
  Edit: 'edit_contract_fuel_type',
  View: 'view_contract_fuel_type',
} as const

export const ContractVehicleType = {
  Create: 'create_contract_vehicle_type',
  Delete: 'delete_contract_vehicle_type',
  Edit: 'edit_contract_vehicle_type',
  View: 'view_contract_vehicle_type',
} as const

export const Contractor = {
  Create: 'create_contractor',
  Delete: 'delete_contractor',
  Edit: 'edit_contractor',
  View: 'view_contractor',
} as const

export const Dashboard = {
  Manage: 'manage_dashboard_targets',
  View: 'view_dashboard',
} as const

export const Discount = {
  Create: 'create_discount',
  Delete: 'delete_discount',
  Edit: 'edit_discount',
  View: 'view_discount',
} as const

export const Document = {
  Create: 'create_document',
  Delete: 'delete_document',
  Edit: 'edit_document',
  View: 'view_document',
} as const

export const Driver = {
  Create: 'create_driver',
  Delete: 'delete_driver',
  Edit: 'edit_driver',
  View: 'view_driver',
} as const

export const DriverDocument = {
  Create: 'create_driver_document',
  Delete: 'delete_driver_document',
  Edit: 'edit_driver_document',
  View: 'view_driver_document',
} as const

export const DriverLicenseType = {
  Create: 'create_driver_license_type',
  Delete: 'delete_driver_license_type',
  Edit: 'edit_driver_license_type',
  View: 'view_driver_license_type',
} as const

export const FuelType = {
  Create: 'create_fuel_type',
  Delete: 'delete_fuel_type',
  Edit: 'edit_fuel_type',
  View: 'view_fuel_type',
} as const

export const IndustryType = {
  Create: 'create_industry_type',
  Delete: 'delete_industry_type',
  Edit: 'edit_industry_type',
  View: 'view_industry_type',
} as const

export const Lead = {
  Create: 'create_lead',
  Delete: 'delete_lead',
  Edit: 'edit_lead',
  View: 'view_lead',
} as const

export const Invoice = {
  Create: 'create_invoice',
  CreatePaymentRequest: 'create_invoice_payment_request',
  Delete: 'delete_invoice_payment_request',
  Edit: 'edit_invoice',
  EditPaymentRequest: 'edit_invoice_payment_request',
  View: 'view_invoice',
} as const

export const ModelHasPermission = {
  Create: 'create_model_has_permission',
  Delete: 'delete_model_has_permission',
  Edit: 'edit_model_has_permission',
  View: 'view_model_has_permission',
} as const

export const ModelHasRole = {
  Create: 'create_model_has_role',
  Delete: 'delete_model_has_role',
  Edit: 'edit_model_has_role',
  View: 'view_model_has_role',
} as const

export const PermissionModule = {
  Create: 'create_permission',
  Delete: 'delete_permission',
  Edit: 'edit_permission',
  Manage: 'manage_permissions',
  View: 'view_permission',
} as const

export const Role = {
  Create: 'create_role',
  Delete: 'delete_role',
  Edit: 'edit_role',
  View: 'view_role',
} as const

export const RoleHasPermission = {
  Create: 'create_role_has_permission',
  Delete: 'delete_role_has_permission',
  Edit: 'edit_role_has_permission',
  View: 'view_role_has_permission',
} as const

export const Sector = {
  Create: 'create_sector',
  Delete: 'delete_sector',
  Edit: 'edit_sector',
  View: 'view_sector',
} as const

export const ServiceRequirement = {
  Create: 'create_service_requirement',
  Delete: 'delete_service_requirement',
  Edit: 'edit_service_requirement',
  View: 'view_service_requirement',
} as const

export const Tax = {
  Create: 'create_tax',
  Delete: 'delete_tax',
  Edit: 'edit_tax',
  View: 'view_tax',
} as const

export const Users = {
  Create: 'create_user',
  Delete: 'delete_user',
  Edit: 'edit_user',
  View: 'view_user',
} as const

export const Trip = {
  AuditTrip: 'audit_trip',
  Cancel: 'cancel_trip',
  CancelInProgress: 'cancel_trip:in_progress',
  Create: 'create_trip',
  CreateHistoricalTrip: 'create_historical_trip',
  Delete: 'delete_trip',
  Edit: 'edit_trip',
  EditCharges: 'edit_charges',
  EditCompleted: 'edit_trip:completed',
  EditInProgress: 'edit_trip:in_progress',
  PickOtherContractVehicle: 'pick_other_contract_vehicle',
  Review: 'review_trip',
  SelfCompleteTrip: 'self_complete_trip',
  View: 'view_trip',
} as const

export const Vehicle = {
  AssignDriver: 'assign_driver',
  Create: 'create_vehicle',
  Delete: 'delete_vehicle',
  Edit: 'edit_vehicle',
  View: 'view_vehicle',
} as const

export const VehicleBrand = {
  Create: 'create_vehicle_brand',
  Delete: 'delete_vehicle_brand',
  Edit: 'edit_vehicle_brand',
  View: 'view_vehicle_brand',
} as const

export const VehicleBrandModel = {
  Create: 'create_vehicle_brand_model',
  Delete: 'delete_vehicle_brand_model',
  Edit: 'edit_vehicle_brand_model',
  View: 'view_vehicle_brand_model',
} as const

export const VehicleCapacityUnit = {
  Create: 'create_vehicle_capacity_unit',
  Delete: 'delete_vehicle_capacity_unit',
  Edit: 'edit_vehicle_capacity_unit',
  View: 'view_vehicle_capacity_unit',
} as const

export const VehicleChangeRequest = {
  Create: 'create_vehicle_change_request',
  Delete: 'delete_vehicle_change_request',
  Edit: 'edit_vehicle_change_request',
  View: 'view_vehicle_change_request',
} as const

export const VehicleFacility = {
  Create: 'create_vehicle_facility',
  Delete: 'delete_vehicle_facility',
  Edit: 'edit_vehicle_facility',
  View: 'view_vehicle_facility',
} as const

export const VehicleFacilityVisit = {
  Enter: 'enter_vehicle_facility_visit',
  Exit: 'exit_vehicle_facility_visit',
} as const

export const VehicleInsurance = {
  Create: 'create_vehicle_insurance',
  Delete: 'delete_vehicle_insurance',
  Edit: 'edit_vehicle_insurance',
  View: 'view_vehicle_insurance',
} as const

export const VehicleType = {
  Create: 'create_vehicle_type',
  Delete: 'delete_vehicle_type',
  Edit: 'edit_vehicle_type',
  View: 'view_vehicle_type',
} as const

export const ActivityLog = {
  View: 'view_activity_log',
} as const

export const NotificationLog = {
  View: 'view_notifications_log',
} as const
