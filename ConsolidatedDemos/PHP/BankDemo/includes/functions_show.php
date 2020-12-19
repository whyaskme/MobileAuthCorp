<?php
//
//  miscellaneous functions
//


// get rid of empty slots in array
function compact_array($array) {
    $new = array();
    foreach($array as $key => $value) {
        if($value != '')
          $new[] = $array[$key];
    }
    return $new;
}  // data validation, cleanup functions

   // functions to Clean up fields

   // remove all but letters, '-' and blanks from field 
   // clears 'name' and other fields of unwanted characters  
  function save_alpha($string) {

      $returnvalue = preg_replace('/[^a-zA-Z &-]/', '', $string);
      return $returnvalue;
  }

   // remove all but letters from field 
   // clears 'name' and other fields of unwanted characters  
  function save_alpha_strict($string) {

      $returnvalue = preg_replace('/[^a-zA-Z]/', '', $string);
      return $returnvalue;
  }
  
  // remove all but letters, numbers, '-' and blanks from field
  // clears 'address' and other fields of unwanted characters
  function save_alphanum($string) {
      $returnvalue = preg_replace('/[^a-zA-Z0-9 &-]/', '', $string);
      return $returnvalue;
  }

  // remove all but letters, numbers, and blanks from field
  // clears 'address' and other fields of unwanted characters
  function save_alphanum_strict($string) {
      $returnvalue = preg_replace('/[^a-zA-Z0-9]/', '', $string);
      return $returnvalue;
  }
  
  // get number amount from dollar.cents formatted - field $1,000.00
  //  currently positive value only
  function save_money_amount($string) {
      $returnvalue = (float)preg_replace("/[^0-9\.]/", "", $string);
      return $returnvalue;
  }

  // verify that field contains only 0-9
function is_number($number)  { 

    $text = (string)$number;
    $textlen = strlen($text);
    if ($textlen==0) return 0;
    for ($i=0;$i < $textlen;$i++)
    { $ch = ord($text{$i});
       if (($ch<48) || ($ch>57)) return 0;
    }
    return 1;
}
  // verify that field contains only 0-9 or .
function is_number_dot($number)
{
    $text = (string)$number;
    $textlen = strlen($text);
    if ($textlen==0) return 0;
    for ($i=0;$i < $textlen;$i++)
    { $ch = ord($text{$i});
       if (($ch<48) || ($ch>57)) {
           if($ch==46) continue;
           else return 0;
       }
    }
    return 1;
}


// verify that field contains only 0-9, or alpha characters
function is_alphanumeric($string) { 

    $text = (string)$string;
    $textlen = strlen($text);
    if ($textlen==0) return 0;
    for ($i=0;$i < $textlen;$i++)
    { $ch = ord($text{$i});
       $ok = 0;
       if (($ch>=48) && ($ch<=57)) $ok = 1;
       if (($ch>=65) && ($ch<=90)) $ok = 1;
       if (($ch>=97) && ($ch<=122)) $ok = 1;

       if($ok <> 1) return 0;
    }
    return 1;
}

// verify that field contains only 0-9, or alpha,or - or .
function is_alphanumeric_dashdot($string) { 

    $text = (string)$string;
    $textlen = strlen($text);
    if ($textlen==0) return 0;
    for ($i=0;$i < $textlen;$i++)
    { $ch = ord($text{$i});
       $ok = 0;
       if (($ch>=48) && ($ch<=57)) $ok = 1;
       if (($ch>=65) && ($ch<=90)) $ok = 1;
       if (($ch>=97) && ($ch<=122)) $ok = 1;
       if($ch == 32 || $ch==46 || $ch ==45) $ok = 1;   // blank, ., or -
       if($ok <> 1) return 0;
    }
    return 1;
}


// verify that field contains only 0-9, alpha, or _ characters
function is_alphanumeric_underscore($string) { 

    $text = (string)$string;
    $textlen = strlen($text);
    if ($textlen==0) return 0;
    for ($i=0;$i < $textlen;$i++)  { 
       $ch = ord($text{$i});
       $ok = 0;
       if (($ch>=48) && ($ch<=57)) $ok = 1;
       if (($ch>=65) && ($ch<=90)) $ok = 1;
       if (($ch>=97) && ($ch<=122)) $ok = 1;
       if ($ch==95) $ok = 1;      // _ character
       if($ok <> 1) return 0;
    }
    return 1;
}
 
// verify that field contains only alpha characters
//  strict = true (only letters), strict = null (letters, blank, hyphen)
function is_alpha($string, $strict = null)  { 
    $blank = 32;
    $hyphen = 45;
    $period = 46;
    $comma = 44;

    $text = (string)$string;
    $textlen = strlen($text);
    if ($textlen==0) return 0;
    for ($i=0;$i < $textlen;$i++)    { 
       $ch = ord($text{$i});
       $ok = 0;
       $other = 0;
       if (($ch>=65) && ($ch<=90)) $ok = 1;
       if (($ch>=97) && ($ch<=122)) $ok = 1;
       if($strict != true)  {
          if ($ch==$blank || $ch==$hyphen || $ch==$period || $ch == $comma) $other = 1;
       }
       if(($ok != 1 &&  $strict == true)
          || ($ok != 1 && ($strict == false && $other != 1))) return 0;
    }
    return 1;
}

// verify that field contains anything except \ ' " & %
function isValidSpecial($string) { 

    $textlen = strlen($string);
    if ($textlen==0) return 0;
    for ($i=0;$i < $textlen;$i++) { 
       $ch = ord($string{$i});
       $ok = 0;
       if ($ch==92 || $ch == 96 || $ch==34 || $ch==37 || $ch==38 || $ch == 39 )  // 92 = \,  96 = `, 34 = ",  37 = %, 39 = ', 38 = &
          return 0;
    }
    return 1;
}

    // search and extract the Search arguments from the url
function getsrx($string, $count) { 
    
    $start = 0;
    for($ii=0; $ii<$count; $ii++)  {  
       $begin = strpos($string, "srx_", $start);
       if($begin === false) return false;
       $start = $begin + 1;
     }
     $end = strpos($string, "&", $begin);
     if($end === false)
        return substr($string, $begin);
     if($end > 1)
       return substr($string, $begin, $end-$begin);
}

// extract email from string, email may be inside <  >
function parseEmail($string) {
    $email = $data = trim($string);
    $bracket1 = strpos($data, '<');
    if($bracket1 !== false) {
        $bracket2 = strpos($data, '>');
        if($bracket2 === false)
           return false;
        $email = substr($data, $bracket1+1, $bracket2-$bracket1-1);   
    }
    $status = isEmail($email);
    if($status == true)
       return $email;
    else
       return false;
}  

function removeBrackets($string) {
   $out1 = str_replace('<', '', $string);
   $out = str_replace('>', '', $out1);
   return $out;
} 

// validate email address
function isEmail($email) {
//    $regex = "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:[A-Z]{2}|com|org|net|edu|gov|mil|biz|info|mobi|name|aero|asia|jobs|museum)\b";

//$regex = '/^(?:(?:(?:[^@,"\[\]\x5c\x00-\x20\x7f-\xff\.]|\x5c(?=[@,"\[\]\x5c\x00-\x20\x7f-\xff]))(?:[^@,"\[\]\x5c\x00-\x20\x7f-\xff\.]|(?<=\x5c)[@,"\[\]\x5c\x00-\x20\x7f-\xff]|\x5c(?=[@,"\[\]\x5c\x00-\x20\x7f-\xff])|\.(?=[^\.])){1,62}(?:[^@,"\[\]\x5c\x00-\x20\x7f-\xff\.]|(?<=\x5c)[@,"\[\]\x5c\x00-\x20\x7f-\xff])|[^@,"\[\]\x5c\x00-\x20\x7f-\xff\.]{1,2})|"(?:[^"]|(?<=\x5c)"){1,62}")@(?:(?!.{64})(?:[a-zA-Z0-9][a-zA-Z0-9-]{1,61}[a-zA-Z0-9]\.?|[a-zA-Z0-9]\.?)+\.(?:xn--[a-zA-Z0-9]+|[a-zA-Z]{2,6})|\[(?:[0-1]?\d?\d|2[0-4]\d|25[0-5])(?:\.(?:[0-1]?\d?\d|2[0-4]\d|25[0-5])){3}\])$/';
// $regex = '/^.+\@(\[?)[a-zA-Z0-9\-\.]+\.([a-zA-Z]{2,3}|[0-9]{1,3})(\]?)$/';
    $regex = '/^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$/';
    $status = preg_match($regex, $email);
    return $status;
}

// phone number
function isPhone($phone) {
    $regex = '/^(?:\([2-9]\d{2}\)\ ?|[2-9]\d{2}(?:\-?|\ ?))[2-9]\d{2}[- ]?\d{4}$/';
    $status = preg_match($regex, $phone);
    return $status;
}

// credit card   - allow 16 digits, or 19 nnnn-nnnn-nnnn-nnnn
function isCreditCard($number) {
    if(!is_number($number)) {
       if(strlen($number) == 19 
         &&($number[4] == '-' && $number[9] == '-' && $number[14] == '-')) {
             $holdnum = str_replace('-', '', $number);
             if(is_number($holdnum)) {
                return true;
             }
             else {
                 return false;
             }
       }
       else if(strlen($number) > 16) {
          return false;
       }
    }
    else if(is_number($number) && strlen($number) == 16) {
       return true;
    }   
       
    return false;
}      


// sort associative array by the value of a specific index/key
function array_sort_on_key($array, $on, $order=SORT_ASC)
{
    $new_array = array();
    $sortable_array = array();

    if (count($array) > 0) {
        foreach ($array as $k => $v) {
            if (is_array($v)) {
                foreach ($v as $k2 => $v2) {
                    if ($k2 == $on) {
                        $sortable_array[$k] = $v2;
                    }
                }
            } else {
                $sortable_array[$k] = $v;
            }
        }

        switch ($order) {
            case SORT_ASC:
                $status = uasort($sortable_array, 'cmp_nocase');
            break;
            case SORT_DESC:
                $status = uasort($sortable_array, 'cmp_nocase_r');
            break;
        }

        foreach ($sortable_array as $k => $v) {
            $new_array[$k] = $array[$k];
        }
    }
    if($status == false)
        return false;
    else
        return $new_array;
}

function array_sort(&$array, $order=SORT_ASC) {
    switch ($order) {
        case SORT_ASC:
           $status = uasort($array, 'cmp_nocase');
        break;
        case SORT_DESC:
            $status = uasort($array, 'cmp_nocase_r');
        break;
        case NUM_ASC:
            $status = uasort($array, 'cmp_num_a');
        break;
        case NUM_DESC:
            $status = uasort($array, 'cmp_num_r');
        break;
    }
    return $status;

}

// compare function for sort
// -  ignore case ascending order
function cmp_nocase($a, $b) {
    $alen = strlen($a);
    $blen = strlen($b);
    $len = min($alen, $blen);
// var_dump("a: ", $a, "b: ", $b);
    return (strncasecmp($a, $b, $len));
}
// -  use case ascending order
function cmp($a, $b) {
    $alen = strlen($a);
    $blen = strlen($b);
    $len = min($alen, $blen);
    return (stnrcmp($a, $b, $len));
}
// - ignore case descending order
function cmp_nocase_r($a, $b) {
    $alen = strlen($a);
    $blen = strlen($b);
    $len = min($alen, $blen);
    return -1 * (strncasecmp($a, $b, $len));
}
// - use case descending order
function cmp_r($a, $b) {
    $alen = strlen($a);
    $blen = strlen($b);
    $len = min($alen, $blen);
    return -1 * (strncmp($a, $b, $len));
}
//  - compare numbers, asc
function cmp_num_a($a, $b) {
    if($a < $b) $ret = -1;
    else if($a > $b) $ret = 1;
    else $ret = 0;
    return $ret;
}
// - compare number, desc
function cmp_num_r($a, $b) {
    if($a < $b) $ret = -1;
    else if($a > $b) $ret = 1;
    else $ret = 0;
    return -1 * $ret;
}
// usort compare function for assoc array
function assoc_cmp($a, $b) {
    return strnatcmp($b['doc_time'], $a['doc_time']);
}
function assoc_cmp2($a, $b) {
    $retval = strnatcmp($a['doc_name'], $b['doc_name']);
   if($retval == 0) return strnatcmp($b['doc_time'], $a['doc_time']);
   return $retval;

}

function makeSortFunction($key1, $asc1, $key2, $asc2) {
        if($asc1 == 'a')
              $code = "\$returnval = strnatcasecmp(\$a['$key1'], \$b['$key1']);";
        else  if($asc1 == 'd')
              $code = "\$returnval = strnatcasecmp(\$b['$key1'], \$a['$key1']);";

        if($key2 != '' && $key2 != null && $asc2 == 'a')
              $code .= " if(\$returnval == 0)  \$returnval = strnatcasecmp(\$a['$key2'], \$b['$key2']);";
        else if($key2 != '' && $key2 != null && $asc2 == 'd')
              $code .= " if(\$returnval == 0)  \$returnval = strnatcasecmp(\$b['$key2'], \$a['$key2']);";

        $code .= " return \$returnval;";
//        var_dump("code: ", $code);
        return create_function('$a,$b', $code);
        }


// sort array of assoc arrays by key - up to two keys - key2 within key1 with 'asc' or 'desc'
// - can sort rows array returned from database
function sortByKey($data, $key1, $asc1, $key2, $asc2) {
    $comparefunc = makeSortFunction($key1, $asc1, $key2, $asc2);
     usort($data, $comparefunc);
     return $data;
   }
   
function sortByKey2($data, $key) {
    function makeSortFunction($key) {
        $code = "return strnatcasecmp(\$a['$key'], \$b['$key'])";
        return create_function('$a$b', $code);
        }
   $comparefunc = makeSortFunction('doc_name');
   usort($data, $companfunc);
   }

// flatten array
//     [0] {'key' => value}  - works for array with only one key/value pair
// to  [0] => value
function flatten($array, $key) {
    $new_array = array();
    $index = 0;
    if(!empty($array)) {
       foreach($array as $inner_array) {
          $new_array[$index] = trim($inner_array[$key]);
          $index++;

       }
    }
    return $new_array;
}
// flatten associative array - 2 dimensions
//   { [0]=> {'k1' => value}, [1] =>{'k2' => value}, ...}  - works for array with only one key/value pair
// to  {'k1' => value, 'k2' => value,, ...  ,...}
 function flatten_assoc_2($array) {
    $new_array = array();
    $index = 0;
   if(!empty($array) && count($array) > 0) {
        foreach($array as $inner_array) {
            $count = 0;
            $newkey = '';
            $newvalue = '';
             foreach($inner_array as $key => $value) {
                 if($count == 0)
                     $newkey = $value;
                 if($count == 1)
                     $newvalue = $value;
                 $count++;
             }
             $new_array[$newkey] = $newvalue;
        }
    }
    return $new_array;
}

// extract from array
//     [key] => {'k1' => value, 'k2' => value, ...}  - works for array with only one key/value pair
// to  {'k1' => value, 'k2' => value, ...}
function getSubArray($array, $key) {
    $new_array = array();
    $index = 0;
   if(!empty($array[$key]) && count($array) > 0) {
        foreach($array as $inner_array) {
          if($index > $key)
              break;
          if($index == $key)
              $new_array = $inner_array;
          $index++;
       }
    }
    return $new_array;
}


// remove duplicate (case insensitive) entry in indexed array
function removeDuplicates($array) {
    $new_array = array();
    $previous = '';
    if(!empty($array)) {
        foreach($array as $key => $value)  {
            $alen = strlen($previous);
            $blen = strlen($value);
            $len  = min($alen, $blen);
            if(strncasecmp($value, $previous, $len) == 0) {
                $previous = $value;
                continue;
            }
            else {
                $new_array[$key] = $value;
                $previous = $value;
            }
        }
    }
    return $new_array;

}

// remove assoc array from array of assoc array's
//   { [0]=> {'k1' => value, 'k2' => value, ...}, [1] =>{'k1' => value}, 'k2' => value, ...}  - works for array with any number of key/value pairs
// removes [n] => {'k1' => value, 'k2' => value,..} for kn == rowKeyValue
// returns new re-indexed array
function removeRowFromArray(&$array, $rowKey, $rowKeyValue) {
    $new_array = array();
    if(!empty($array)) {
        foreach($array as $index => $row) {
            if($row[$rowKey] == $rowKeyValue)
                continue;
            $new_array[] = $row;
        }
    }
    else {
        return $array;
    }
    return $new_array;
}

// remove assoc array from array of assoc array's
//   { [0]=> {'k1' => value, 'k2' => value, ...}, [1] =>{'k1' => value}, 'k2' => value, ...}  - works for array with any number of key/value pairs
// removes [n] => {'k1' => value, 'k2' => value,..} for kn == rowKeyValue
// just unsets row, so array is not re-indexed
function unsetRowFromArray(&$array, $rowKey, $rowKeyValue) {
    $atatus = false;
    if(!empty($array)) {
        foreach($array as $index => $row) {
            if($row[$rowKey] == $rowKeyValue) {
                unset($array[$index]);
                $status = true;
            }
        }
    }
    return $status;
}

// array merge keeping keys intact
function array_merge_keepkeys ($arr1, $arr2) {
    $keys1 = array_keys($arr1);
    $keys2 = array_keys($arr2);
    $values1 = array_values($arr1);
    $values2 = array_values($arr2);
    $newkeys = array_merge($keys1, $keys2);
    $newvalues = array_merge($values1, $values2);
    $i = 0;
    foreach($newkeys as $key) {
        $strkey = (string)$key;
        $outarr[$strkey] = $newvalues[$i];
        $i++;
    }
    return($outarr);
    
}

// clean up strings for use in sql insert, update, or searches
// - leave quotes
function cleanUpForSQL($string) {
   $temp = stripslashes($string);
   $temp = trim(strip_tags($temp));
   $temp = str_replace('%', '', $temp);
   $temp = str_replace("'", "", $temp);
   return $temp;
}

// clean up strings for use in sql insert, update, or searches
// - remove quotes
function removeQuotes($string) {
   $temp = str_replace('"', '', $string);
   $temp = str_replace("'", "", $temp);
   return $temp;
}

// create random string - used for auth code
function random_string($length) {
    $base='AaBbCcDdEeFfGgHhKkLMmNqPpQqRrSsTtWwXxYyZz0123456789';  // 51 chars, max length
    $max=strlen($base)-1;
    if($length > $max +1)
       $length = $max + 1;
    if($length < $max +1)
       $maxIndex = $max - $length + 1;
    else
       $maxIndex = 0; 
    $activatecode='';                 
//    mt_srand((double)(microtime() ^ posix_getpid()));
    mt_srand((double)microtime()*37593);   // multiply by odd number
    while (strlen($activatecode)<$max+1)
       $activatecode.=$base{mt_rand(0,$max)};   // create random string of 51 chars
  
   if($maxIndex > 0)
       $index = mt_rand(0, $maxIndex);
   else
       $index = 0;    
   return substr($activatecode, $index, $length);   // return substring of 'length' chars starting at random index
}   

    
    // return number with 'mask' except for last 'length' digits
    function maskNumber($number, $mask, $length) {
        $len = strlen($number);
        for($i = 0; $i < $len-$length; $i++) {
            $out[] = $mask;
        }
        for($i = $len-$length; $i < $len; $i++) {
            $out[] = $number[$i];
        }
        return implode($out);    // return as string
    }
    
    function ellipsis($string, $length) {
        if(strlen($string) <= $length)
           return $string;
        else 
           return substr($string, 0, $length-3).'...'; 
    }  

  // convert string to hex
  function strToHex($string){
      $hex='';
      for ($i=0; $i < strlen($string); $i++){
          $ord = ord($string[$i]);
          $hex .= sprintf('%02.x', $ord);
      }
      return $hex;
  }  
  
  // convert hex to string
  function hexToStr($hex) {
     $string='';
     for ($i=0; $i < strlen($hex)-1; $i+=2) {
        $string .= chr(hexdec($hex[$i].$hex[$i+1]));
     }
     return $string;
}     
  
  // convert simpleXML object to array  - so can use in SESSION variable
  function xmlToArray($xml) {
     $array = json_decode(json_encode($xml), TRUE);
    
     foreach ( array_slice($array, 0) as $key => $value ) {
        if ( empty($value) ) $array[$key] = NULL;
          elseif ( is_array($value) ) $array[$key] = xmlToArray($value);
     }
     return $array;
  }
  
  // get select option attribute - for display of select list
  function getAttributeValue($myAttr, $string) {
    $chunks = preg_split('/\s+/', $string);
    foreach($chunks as $chunk) {
       $attrList = explode('=', $chunk);
       if($attrList[0] == $myAttr)  {
          $myValue = trim($attrList[1], "'\"");
          return $myValue;
       }
    }
    return false;
}

  


?>