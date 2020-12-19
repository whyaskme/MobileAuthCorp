<?php
  // validation routines         type  =>  function name
class Validate {
    
    public $validationTypes = array('text' => 'is_alphanumeric_dashdot',
                                   'textSpecial' => 'isValidSpecial',
                                   'state' => 'validState',
                                   'zip' => 'postalCode',
                                   'email' => 'isEmail',
                                   'phone' => 'isPhone',
                                   'number' => 'is_number',
                                   'creditcard' => 'isCreditCard');
                                   
    public $errorArray = array();                               

    // validate and update $fieldArray with data to save
    // update and return $errorArray with errors, or empty                               
    public function validateUserFields($fieldArray, $validFields, $requiredFields) {
         if(isset($fieldArray['password']) && $fieldArray['password'] != '') {
              $passwordOK = true;
              if(!isValidSpecial($fieldArray['password']))  {   // is valid password with special characters
                  $this->errorArray[] = "Invalid character in password - ',  \", &, or  \\, not allowed";
                  $passwordOK = false;
              }
              else if($fieldArray['password'] != $fieldArray['password2']) {
                  $this->errorArray[] = 'New passwords do not match';
                  $passwordOK = false;
              }
              // save if valid
              if($passwordOK) {
                  $fieldArray['password'] = md5($fieldArray['password']);
                  $fieldArray['password_encrypted'] = 1;
              }
       
          } 
          foreach($fieldArray as $field => $value) {
             if(in_array($field, array_keys($validFields))) {
                 $validateType = $validFields[$field];  // get validation type
                 if($validateType == null)     // no validation for this field
                     continue;
                 $validateFunction = $this->validationTypes[$validateType];
                 // do validation only if has a value and function exists
                 if($value != '' && $validateFunction != null && $validateFunction($value) != true) {
                    //$this->errorArray[] = "'$field' is invalid";
                     $this->errorArray[] = "Invalid Login";
                 }
             }
          }
          
          foreach($requiredFields as $field) {
              if($fieldArray[$field] == null || $fieldArray[$field] == '')
                 //$this->errorArray[] = "'$field' is required";
                  $this->errorArray[] = "Invalid Login";
          }
          
          if(empty($this->errorArray))
             return array('status' => true, 'errorArray' => $this->errorArray);  // validated ok
          else
             return array('status' => false, 'errorArray' => $this->errorArray);   // errors, return messages
          
    }
}

?>
