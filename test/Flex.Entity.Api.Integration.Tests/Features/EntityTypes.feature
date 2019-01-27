Feature: EntityTypes
	In order to manage entitytypes in flex
	As an OE user
	I want to be be able to create, delete and view entity types.

# Legends (Annotations)
# PAT - ProductAcceptanceTest
# NAT - NegativeAcceptanceTest
# TAT - TechnicalAcceptanceTest

@PAT
Scenario: ET1 GET Entity Types - Get entity type collections 
	Given a request for all entity types
		| Name          | Value                                 |
		| RelativeUrl   | /entities/types                       |
		| Accept        | application/json;						|
	When the client makes a GET request
	Then the Api returns with response code '200'
	And the response contains a collection of entity type details


@PAT
Scenario: ET2 GET Entity Types - Get entity type  
	Given a request for an entity types with prefix 'C'
		| Name          | Value                                 |
		| RelativeUrl   | /entities/types						|
		| Accept        | application/json;						|
	When the client makes a GET request
	Then the Api returns with response code '200'
	And the response contains entity type details

@PAT
Scenario: ET3 GET Entity Types - entity type not found
	Given a request for an entity types with prefix 'Z'
		| Name          | Value                                 |
		| RelativeUrl   | /entities/types						|
		| Accept        | application/json;						|
	When the client makes a GET request
	Then the Api returns with response code '404'
	And the response contains error details

@PAT
@EntityType
Scenario: ET4 Create Entity Type - create new Entity Type
	Given a request to create a new entity type with prefix 'X'
		| Name          | Value                                 |
		| RelativeUrl   | /entities/types						|
		| Accept        | application/json;						|
	When the client makes a POST request with the body '{ prefix : "X", name : "Xtreem Entity", allow_in_asset_hierarchy : true, allow_in_service_hierarchy : false, allow_same_type_descendant : false}'
	Then the Api returns with response code '201'
	And the response contains the newly created entity type details.

@PAT
@EntityType
Scenario: ET5 Create Entity Type - create invalid Entity Type
	Given a request to create a new entity type with prefix 'X'
		| Name          | Value                                 |
		| RelativeUrl   | /entities/types						|
		| Accept        | application/json;						|
	When the client makes a POST request with the body '{ prefix : "X"}'
	Then the Api returns with response code '400'
	And the response contains the error details for the BadRequest
@PAT
Scenario: ET6 Delete Entity Type - delete entity type 
	Given a request to delete an entity types with prefix 'X'
		| Name          | Value                                 |
		| RelativeUrl   | /entities/types						|
		| Accept        | application/json;						|
	When the client makes a DELETE request
	Then the Api returns with response code '200'
	And the response is empty

@PAT
Scenario: ET7 Get Entity Type - get entity type without authorization
	Given a request for an entity types with prefix 'C'
		| Name          | Value                                 |
		| RelativeUrl   | /entities/types						|
		| Accept        | application/json;						|
	When the client makes a GET request without a bearer token
	Then the Api returns with response code '401'
	And the response contains an error message

@PAT
Scenario: ET8 Get Entity Type - get entity type without emailId in the bearer token
	Given a request for an entity types with prefix 'C'
         | Name            | Value                |
         | RelativeUrl     |  /entities/types     |
		 | Accept          | application/json;	  |
	When the Client makes a Get request with a bearer token having no emailid
	Then the Api returns with response code '401'
	And the response contains an error message

@PAT
Scenario: ET9 Get Entity Type : get entity type without permission present in token
	Given a request for an entity types with prefix 'C'
		 | Name            | Value                |
         | RelativeUrl     |  /entities/types     |
		 | Accept          | application/json;	  |
	When the Client makes a Get request with a bearer token having no permission
	Then the Api returns with response code '200'
	And the response contains entity type details
	