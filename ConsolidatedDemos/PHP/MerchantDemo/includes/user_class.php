<?php

abstract class UserStore {
// base class for subsequent user_id access and processing classes

public $user_id='';     
public $password='';    
public $groupid='';      
public $fname='';        
public $lname='';        
public $data='';         
public $sessionid= '';   
public $email='';   
public $rights='';    

function get_user_id(){} 
function set_userid($value){} 
function get_password(){} 
function set_password($value){} 
function get_groupid(){} 
function set_groupid($value){} 
function get_fname(){} 
function set_fname($value){} 
function get_lname(){} 
function set_lname($value){} 
function get_data(){} 
function set_data($value){} 
function get_sessionid(){} 
function set_sessionid($value){} 
function get_email(){}
function set_email($value){} 

}

// class for processing userids stored in database table "users"
// must include 'db_class.php' in source file where this is used
class User_DB extends UserStore{
 public $DB; 
 public $id;
 public $user_id;  

 public $userlist = array();

// instantiate user, load if creating with userid, or just create empty new empty user
function __construct($user_id='') {
     $this->DB = new DbClass('localhost', 'mac_test', 'mac_user', 'mac123', 'standalone');
    if($user_id != '') {
     $this->user_id = $user_id;
     $row = $this->getUserByUserID($user_id);
     if($row == false)
        return null;
     $this->id = $row['id'];
     $this->fname = $row['fname'];
     $this->lname = $row['lname'];
     $this->email = $row['email'];
     $this->shared_userid = $row['shared_userid'];
     $this->user_timezone = $row['user_timezone'];
     $this->state = $row['state'];
     $this->country = $row['country'];
  }
}

function __destruct()
{
}   

// get and set functions for each field in class
public function set_userid($user_id){ 
    $this->user_id = $user_id; 
 //   $this->updateuserfield('user_id', $user_id);
}
public function set_password($password){ 
    $this->password = $password; 
    $this->updateuserfield('password', $password);
}
public function set_groupid($group){ 
    $this->groupid = $group; 
    $this->updateuserfield('groupid', $group);
}
public function set_fname($fname){ 
    $this->fname = $fname; 
    $this->updateuserfield('fname', $fname);
}
public function set_lname($lname){ 
    $this->lname = $lname; 
    $this->updateuserfield('lname', $lname);
}
public function set_data($data){ 
    $this->data = $data; 
    $this->updateuserfield('data', $data);
}
public function set_sessionid($sessionid){ 
    $this->sessionid = $sessionid; 
    $this->updateuserfield('sessionid', $sessionid);
}
public function set_email($email){ 
    $this->email = $email; 
    $this->updateuserfield('email', $email);
}

public function get_userid(){ 
     $this->user_id = $this->getuserfield('user_id');
     return($this->user_id);
}
public function get_password(){ 
    $this->password = $this->getuserfield('password');
    return $this->password;
}
public function get_groupid(){ 
    $this->groupid = $this->getuserfield('groupid');
    return $this->groupid;
}
public function get_fname(){ 
    $this->fname = $this->getuserfield('fname');
    return($this->fname);
}
public function get_lname(){ 
    $this->lname = $this->getuserfield('lname');
    return($this->lname);
}
public function get_data(){ 
    $this->data = $this->getuserfield('data');
    return($this->data);
}
public function get_sessionid(){ 
    $this->sessionid = $this->getuserfield('sessionid');
    return($this->sessionid);
}
public function get_email(){ 
    $this->email = $this->getuserfield('email');
    return($this->email);
}

// load common fields passed userid
function loaduser($user_id) {
  if($this->get_user_id() == null)  
     return null;  
  $this->get_password();  
  $this->get_groupid();  
  $this->get_fname();  
  $this->get_lname();  
  $this->get_email();  
  $this->get_sessionid();  
  $this->get_data();  
}

function getUserByActivateCode($code) {
   $query = "SELECT
                *
              FROM users
              WHERE activate_code='$code'
              ";
   if(!($result =  mysqli_query($this->DB->connection, $query)))
    {
    $this->DB->showerror();
    }
   $rowsFound = mysqli_num_rows($result);
   $row = @ mysqli_fetch_assoc($result);
   if($rowsFound == 1) { 
      return $row;
   }
   else  return FALSE; 
}

// is user activated - activate_status = 0
function isUserReadytoActivate() {
    if($this->user_id == '')
        return false;
    else if($this->getuserfield('activate_status') == 1)    
        return true;
    else
        return false;    
}        

// is user already activated
function isUserActivated() {
    if($this->user_id == '')
        return false;
    else if($this->getuserfield('activate_status') == 0)    
        return true;
    else
        return false;    
}        

function activateUser() {
    if($this->user_id == null || $this->user_id == '')
       return false;
    $status = $this->updateuserfield('activate_status', 0);
    return $status;
}
 
 // get active user info by user id
 function getUserByUserID($user_id) {
    $user_id = mysqli_escape_string($this->DB->connection, $user_id);

    $query = "SELECT
                *
              FROM users
              WHERE user_id='$user_id' 
              ";
   if(!($result =  mysqli_query($this->DB->connection, $query)))
     {
    $this->DB->showerror();
     }
   $rowsFound = mysqli_num_rows($result);
   $row = @ mysqli_fetch_assoc($result);
   if($rowsFound == 1) { 
       return $row;
   }
   else  return false; 
}

// get user data based on condition  -  userid not known
function getUserData($condition) {
    if(!is_array($condition))
       return false;
    $count = 0;  
    foreach($condition as $field => $value) {
        if($count > 0)
            $whereCondition .= " and ";
        $cleanedValue = mysqli_escape_string($this->DB->connection, $value);
        $whereCondition = " $field='$cleanedValue' "; 
        $count++;
    }
    if(strlen($whereCondition) > 1)
        $whereClause = " WHERE ".$whereCondition;
    else
        $whereClause = '';
        
    $query = "SELECT *
              FROM users
              $whereClause";
  
    if(!($result =  mysqli_query($this->DB->connection, $query)))
       {
       $this->DB->showerror();
       }
    $rowsFound = mysqli_num_rows($result);
    $row = @ mysqli_fetch_assoc($result);
    if($rowsFound == 1) {
       $this->user_id = $row['user_id'];  // set object to  point to this user
       return $row;
    }
    else  return FALSE; 
}
                    
        
// retreive array of users in db
function get_userlist()
{
  $query = "SELECT * from users ";
  
  if(!($result =  mysqli_query($this->DB->connection, $query)))
    {
    $this->DB->showerror();
    }
  while( $row = @ mysqli_fetch_assoc($result))
    {
    $this->userlist[] = $row['user_id'];
   }
return $this->userlist;
}

// does userid match entry in db  
function usermatch($in_userid)
{
  $in_userid = mysqli_escape_string($this->DB->connection, $in_userid);
  //$this->user_id = $in_userid;
  $query = "SELECT * from users where user_id='" . $in_userid . "' ";
  
  if(!($result =  mysqli_query($this->DB->connection, $query)))
    {
    $this->DB->showerror();
    }
  $rowsFound = mysqli_num_rows($result);
  $row = @ mysqli_fetch_assoc($result);
  if($rowsFound == 1) { 
      return TRUE;
  }
  else  return false; 
}

// does password match current user
function passwordmatch($in_userid, $in_password)
{
  $in_userid = mysqli_escape_string($this->DB->connection, $in_userid);
  $in_password = mysqli_escape_string($this->DB->connection, $in_password);
  $gotpasswordflag = 0;

  $query = "SELECT * from users where user_id='" . $in_userid . "' ";
  
  if(!($result =  mysqli_query($this->DB->connection, $query)))
    {
    $this->DB->showerror();
    }
  $rowsFound = mysqli_num_rows($result);
  $row = @ mysqli_fetch_assoc($result);
  if($row['password_encrypted'] == 1) {
     $in_password = md5($in_password);
  }
  if($rowsFound == 1) { 
     if($in_password == $row['password']) {
	      $gotpasswordflag = 1;   // got a match
     }
  }
  if($gotpasswordflag == 0)
	 return FALSE; 
  else
	 return TRUE;
}

// delete user
function deleteuser($in_userid)
{
  $in_userid = mysqli_escape_string($this->DB->connection, $in_userid);

  if($this->usermatch($in_userid))
   {
   $query = "DELETE from users where user_id=\"" . $in_userid . "\" ";
  
   if(!($result =  mysqli_query($this->DB->connection, $query)))
     {
    $this->DB->showerror();
     }
   $rowsFound = @ mysqli_affected_rows($this->DB->connection);
   if($rowsFound == 1)
     return TRUE;
   else
     return FALSE;	 		   
   }
  else 
    return FALSE;  
	
}

// change password for current user
function changepassword($in_userid, $in_newpwd)
{
  $in_userid = mysqli_escape_string($this->DB->connection, $in_userid);
  $in_newpwd = mysqli_escape_string($this->DB->connection, $in_newpwd);
  if($this->usermatch($in_userid))
   {
   $query = "UPDATE users SET " .
              "password = \"" . crypt($in_newpwd, $in_userid) . "\" " .
			  "WHERE user_id= \"" . $in_userid . "\" ";
   if(!($result =  mysqli_query($this->DB->connection, $query)))
     {
     $this->DB->showerror();
     }
   $rowsFound = @ mysqli_affected_rows($this->DB->connection);
   if($rowsFound >= 0) {  // must include 0 since mysql won't UPDATE if new password is same as what's in db
     $password = $in_newpwd;
     return TRUE;
     }
   else
     return FALSE;	 		   
   }
  else 
    return FALSE;  


}

// see if this session is already in db
function sessionmatch($in_sessionid) {
  
  $gotsessionflag = 0;  
  $query = "SELECT * from users where sessionid=\"" . $in_sessionid . "\" ";
  
  if(!($result =  mysqli_query($this->DB->connection, $query)))
    {
    $this->DB->showerror();
    }
  $rowsFound = mysqli_num_rows($result);
  $row = @ mysqli_fetch_assoc($result);
  $sessionid = $row['sessionid'];
  if($rowsFound == 1) { 
     if($in_sessionid == $sessionid) {
          $gotsessionflag = 1;   // got a match
    }
  } 
  if($gotsessionflag == 0)
     return FALSE; 
  else
     return TRUE;
     
}     
     

 // update field value to db for current user
function updateuserfield($field, $value) {
   
    $value = mysqli_escape_string($this->DB->connection, $value);

     // find all columns to make sure fields in Fieldarray match the table    
   $fieldList = $this->getFieldList('users');
    
    // if user not in database then 'add' if field is 'userid'    
   if(!$this->usermatch($this->user_id)) {  
       if($field == 'user_id') {
           $query = "INSERT INTO users SET " .
                "user_id='$value'";
           if(!($result =  mysqli_query($this->DB->connection, $query))) {
               $this->DB->showerror();
              }
           $rowsFound = @ mysqli_affected_rows($this->DB->connection);
           if($rowsFound == 1) { 
              return TRUE;
              }
           else
             return FALSE;   
       } 
       else
         return FALSE;
    }
                 
    // user is in database so update other fields
    if(in_array($field, $fieldList)) {
    $updateset = "$field='$value'";    
    $query = "UPDATE users SET " .
                $updateset .
                " WHERE user_id='$this->user_id'";
                
   if(!($result =  mysqli_query($this->DB->connection, $query)))
     {
     $this->DB->showerror();
     }
   $rowsFound = @ mysqli_affected_rows($this->DB->connection);
   if($rowsFound == 1) {     
     return TRUE;
     }
   else
     return FALSE;                
 }
}
                
 // get field value from db for current user
function getuserfield($field) {
    $fieldList = $this->getFieldList('users'); 
    if(!$this->usermatch($this->user_id))
       return NULL;
    
    if(in_array($field, $fieldList)) {
       $query = "SELECT $field FROM users " .
                " WHERE user_id='$this->user_id'";
                
   if(!($result =  mysqli_query($this->DB->connection, $query)))
     {
     $this->DB->showerror();
     }
   $rowsFound = @ mysqli_affected_rows($this->DB->connection);
   if($rowsFound == 1) { 
     $row = @ mysqli_fetch_assoc($result);
     return $row[$field];
     }
   else
     return NULL;                
 }
}

function getFieldList($tablename) {
        // find all columns to make sure fields in Fieldarray match the table    
    $query = "SHOW COLUMNS from $tablename";
    if(!($result = @ mysqli_query($this->DB->connection, $query)))
       $this->DB->showerror($query);
    $rowsFound = @ mysqli_num_rows($result);
    
    for($i=0; $i<$rowsFound; $i++) {        
        $row[] = @ mysqli_fetch_assoc($result);
        $columns[] = $row[$i]['Field']; 
    }
    if(!empty($columns)) 
        return $columns;
    else
        return false;
}    

                
				
}   ////////// end of User_DB class

// 
//  extend class for Client Managed users
//
class ClientUser extends User_DB {
    
    public $account_id;
    public $group_id;
    
function __construct($user_id='', $account_id='') {  
   User_DB::__construct($user_id);
   $this->account_id = $account_id;
}        
  
// get config values  
function getConfig() {
    $condition['id'] = 1;      // there's only one row for now
    $row = $this->DB->getRow('config', $condition);
    if($row != null)
       return $row;
    else
       return false;
       
}
    
// create new user if no condition, update existing user if have condition
function saveUser($fieldArray, $condition=null) {
    $status = $this->DB->saveRow('users', $fieldArray, $condition);
    return $status;
}

// validation routines         type  =>  function name
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
function validateUserFields($fieldArray, $validFields, $requiredFields) {
     if(isset($fieldArray['password']) && $fieldArray['password'] != '') {
          $passwordOK = true;
          if(!isValidSpecial($fieldArray['password']))  {   // is valid password with special characters
              $this->errorArray[] = "invalid character in password - ',  \", &, or  \\, not allowed";
              $passwordOK = false;
          }
          else if($fieldArray['password'] != $fieldArray['password2']) {
              $this->errorArray[] = 'new passwords do not match';
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
                $this->errorArray[] = "'$field' is invalid";
             }
         }
      }
      
      foreach($requiredFields as $field) {
          if($fieldArray[$field] == null || $fieldArray[$field] == '')
             $this->errorArray[] = "'$field' is required";
      }
      
      if(empty($this->errorArray))
         return array('status' => true, 'errorArray' => $this->errorArray);  // validated ok
      else
         return array('status' => false, 'errorArray' => $this->errorArray);   // errors, return messages
      
}

} // end of ClientUser class

     
?>