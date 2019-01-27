Feature: TagTypes
	In order to manage in Tag Type in Flex
	As a OE user
	I want to be able to create tag type and delete tag type

# Legends (Annotations)
# PAT - ProductAcceptanceTest
# NAT - NegativeAcceptanceTest
# TAT - TechnicalAcceptanceTest

@PAT
Scenario: TT01 Create a Tag Type
	Given a request to create a new tag type with tag type name 'ipaddress'
		| Name          | Value                                 |
		| RelativeUrl   | /entities/tags/                       |
		| Accept        | application/json;						|
	When the client makes a POST request with the body '{ key : "ipaddress"}'
	Then the Api returns with response code '201'
	And the response contains the newly created tags type details

@NAT
Scenario: TT02 Create Tag Type which is already exist in Flex 
	Given a request to create a new tag type with tag type name 'ipaddress'
		| Name          | Value                                 |
		| RelativeUrl   | /entities/tags/                       |
		| Accept        | application/json;						|
	When the client makes a POST request with the body '{ key : "ipaddress"}'
	Then the Api returns with response code '400'	
	And the response contains sucess 'false' with error message 'key = ipaddress already exisits'

@PAT
Scenario: TT03 Delete Tag Type which exist without attached with entities
	Given a a request to delete a tag type with name 'ipaddress'
		| Name          | Value                                 |
		| RelativeUrl   | /entities/tags/                       |
		| Accept        | application/json;						|
	When the client makes a DELETE request
	Then the Api returns with response code '200'
	And the response contains sucess 'true' with error message ''
	

@NAT
Scenario: TT04 Delete Tag Type without exist
	Given a a request to delete a tag type with name 'X'
		| Name          | Value                                 |
		| RelativeUrl   | /entities/tags/                       |
		| Accept        | application/json;						|
	When the client makes a DELETE request
	Then the Api returns with response code '404'
	And the response contains sucess 'false' with error message 'tag type with key = X not found.'

@NAT
@ignore /* Not implemented */
Scenario: TT05 Delete Tag Type which uses in entities
	Given a a request to delete a tag type with name 'hostid'
		| Name          | Value                                 |
		| RelativeUrl   | /entities/tags/                       |
		| Accept        | application/json;						|
	When the client makes a DELETE request
	Then the Api returns with response code '404'
	And the response contains sucess 'false' with error message 'Not found'

	
@PAT
Scenario: TT06 GET Tag Types - Get tag type collections 
	Given a request for all tag types
		| Name          | Value                                 |
		| RelativeUrl   | /entities/tags                       |
		| Accept        | application/json;						|
	When the client makes a GET request
	Then the Api returns with response code '200'
	And the response contains a collection of tag type details


@PAT
Scenario: TT07 GET Tag Types - Get tag type  
	Given a request for an tag types with prefix 'temp'
		| Name          | Value                                 |
		| RelativeUrl   | /entities/tags						|
		| Accept        | application/json;						|
	When the client makes a GET request
	Then the Api returns with response code '200'
	And the response contains tag type details